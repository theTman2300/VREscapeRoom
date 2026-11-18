using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class LaserKey : MonoBehaviour
{
    [SerializeField] LayerMask interactionMask;
    [SerializeField] Transform keyParent;
    [SerializeField] Transform key;
    [Space]
    [Tooltip("Boxcast for hand will start this far below the button.\nThis can help avoid double presses when player pushes too far.")]
    [SerializeField] float downExtends = 0.12f;
    [Tooltip("The key cannot be pressed down further than this")]
    [SerializeField] float minimumKeyPressPercent = 0.1f;
    [SerializeField, Range(0, 1)] float inputDepthPercent = 0.65f;
    [SerializeField, Range(0, 1)] float depressPercent = 0.5f;
    [SerializeField, Range(0, 1)] float smoothing = 0.1f;
    [SerializeField] char keyCharacter = 'a';
    [SerializeField] TextMeshPro characterText;
    [Space]
    [SerializeField] UnityEvent<string> OnKeyPressed = new();

    bool isTouchingKey = false;
    bool isPressed = false;
    float keyHeight = 0.09771223f; //value was for debugging only, this is set in start() to be accurate

    private void Start()
    {
        keyHeight = key.localScale.y;
        characterText.text = keyCharacter.ToString().ToLower();
        GetComponent<BoxCollider>().size = new(key.localScale.x, GetComponent<BoxCollider>().size.y, key.localScale.z);
    }

    private void Update()
    {
        if (!isTouchingKey)
        {
            keyParent.localScale = new(keyParent.localScale.x, Mathf.Lerp(keyParent.localScale.y, 1, 1 - smoothing), keyParent.localScale.z);
            return;
        }

        if (Physics.BoxCast(keyParent.position - new Vector3(0, downExtends, 0), new(key.localScale.x, .01f, key.localScale.z), Vector3.up, out RaycastHit hit, key.rotation, keyHeight + downExtends + 0.005f, interactionMask))
        {
            if (hit.point.y < keyParent.position.y && !isPressed) return; //avoid button pressing down when hand is put underneath it

            float pressPercent = Mathf.Clamp((hit.point.y - key.position.y) / keyHeight, minimumKeyPressPercent, 1);
            keyParent.localScale = new(keyParent.localScale.x, Mathf.Lerp(keyParent.localScale.y, pressPercent, 1 - smoothing), keyParent.localScale.z);

            if (1 - pressPercent >= inputDepthPercent)
            {
                //key pressed
                if (!isPressed)
                {
                    isPressed = true;
                    OnKeyPressed.Invoke(keyCharacter.ToString().ToLower());
                }
            }
            else if (1 - pressPercent <= depressPercent)
            {
                isPressed = false;
            }
        }
        else
        {
            isPressed = false;
            keyParent.localScale = new(keyParent.localScale.x, Mathf.Lerp(keyParent.localScale.y, 1, 1 - smoothing), keyParent.localScale.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isTouchingKey = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTouchingKey = false;
        isPressed = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(keyParent.position - new Vector3(0, downExtends, 0), new(key.localScale.x, .01f, key.localScale.z));
        Gizmos.DrawWireCube(keyParent.position + new Vector3(0, keyHeight + downExtends + 0.005f, 0), new(key.localScale.x, .01f, key.localScale.z));
    }
}
