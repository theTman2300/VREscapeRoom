using UnityEngine;
using UnityEngine.Events;

public class LaserKey : MonoBehaviour
{
    //raycast up only on layer laserkeyboard to find height of interaction
    //use own y but scale parent transform because that is at base of cube

    [SerializeField, Range(0, 1)] float inputDepthPercent = 0.65f;
    [SerializeField] char keyCharacter = 'a';
    [Space]
    [SerializeField] UnityEvent<string> OnKeyPressed = new();

    bool isTouchingKey = false;
    bool isPressed = false;
    float keyHeight;

    private void Start()
    {
        keyHeight = transform.localScale.y;
    }

    private void Update()
    {
        if (!isTouchingKey) return;
        if (Physics.Raycast(transform.parent.position, Vector3.up, out RaycastHit hit, keyHeight, gameObject.layer))
        {
            float pressPercent = Mathf.Clamp01((hit.point.y - transform.position.y) / keyHeight);
            transform.parent.localScale = new(transform.parent.localScale.x, pressPercent, transform.parent.localScale.z);

            if (1 - pressPercent >= inputDepthPercent)
            {
                //key pressed
                if (!isPressed)
                {
                    isPressed = true;
                    OnKeyPressed.Invoke(keyCharacter.ToString().ToLower());
                }
            }
            else
            {
                isPressed = false;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isTouchingKey = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTouchingKey = false;
    }
}
