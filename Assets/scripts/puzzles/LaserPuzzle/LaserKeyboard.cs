using TMPro;
using UnityEngine;

public class LaserKeyboard : MonoBehaviour
{
    [SerializeField] string code = "turnoff";
    [Space]
    [SerializeField] TextMeshPro displayText;
    [SerializeField] string emptyText = "---------------------";
    [SerializeField] int maxCharacterCount = 9;

    string currentInput = "";

    private void Start()
    {
        displayText.text = emptyText;
    }

    public void KeyPressed(string keyCharacter)
    {
        if (currentInput.Length >= maxCharacterCount) return;

        currentInput += keyCharacter;
        displayText.text = currentInput;

        print("Current input: " + currentInput);
    }

    public void CheckCode()
    {
        if (currentInput == code)
        {

        }
        else
        {
            currentInput = "";
            displayText.text = emptyText;
        }
    }
}
