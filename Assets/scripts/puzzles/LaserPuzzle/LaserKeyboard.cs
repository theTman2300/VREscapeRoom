using TMPro;
using UnityEngine;

public class LaserKeyboard : MonoBehaviour
{
    [SerializeField] string code = "turnoff";
    [Space]
    [SerializeField] TextMeshPro displayText;
    [SerializeField] string emptyText = "----";
    [SerializeField] int maxCharacterCount = 10;

    public void KeyPressed(string keyCharacter)
    {

    }

    public void CheckCode()
    {

    }
}
