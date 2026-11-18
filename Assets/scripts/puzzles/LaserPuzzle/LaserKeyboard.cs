using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LaserKeyboard : MonoBehaviour
{
    [SerializeField] bool skipPuzzle = false;
    [SerializeField] string code = "turnoff";
    [Space]
    [SerializeField] TextMeshPro displayText;
    [SerializeField] string emptyText = "---------------------";
    [SerializeField] int maxCharacterCount = 9;
    [Space]
    [SerializeField] GameObject panel;
    [SerializeField] float panelExplosionLinearForce;
    [SerializeField] Vector3 panelExplosionRotationForce;
    [SerializeField] UnityEvent OnCorrectCode = new();

    string currentInput = "";
    bool keyboardActive = true;

    private void Start()
    {
        if (skipPuzzle)
        {
            Debug.Log("laser control skip puzzle active");
        }
        displayText.text = emptyText;
    }

    public void KeyPressed(string keyCharacter)
    {
        if (!keyboardActive) return;
        if (currentInput.Length >= maxCharacterCount) return;

        currentInput += keyCharacter;
        displayText.text = currentInput;

        print("Current input: " + currentInput);
    }

    public void CheckCode()
    {
        if (!keyboardActive) return;

        if (skipPuzzle)
        {
            CorrectCode();
            return;
        }

        if (currentInput == code)
        {
            CorrectCode();
        }
        else
        {
            currentInput = "";
            displayText.text = emptyText;
        }
    }

    void CorrectCode()
    {
        displayText.color = Color.black;
        keyboardActive = false;
        Rigidbody panelRb = panel.GetComponent<Rigidbody>();
        panelRb.isKinematic = false;
        panelRb.AddForce(panel.transform.forward * panelExplosionLinearForce, ForceMode.Impulse);
        panelRb.AddTorque(panelExplosionRotationForce, ForceMode.Impulse);
        OnCorrectCode.Invoke();
    }
}
