using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RotaryLock : MonoBehaviour
{
    [SerializeField] int combintion = 5063;
    [SerializeField] Color correctColor;
    [Space]
    [SerializeField] GameObject rotating;
    [SerializeField] GameObject door;
    [SerializeField] GameObject diamond;
    [SerializeField] TextMeshPro[] displayTexts;

    HingeJoint joint;
    bool lockEnabled = true;

    bool isTurning = false;
    int currentDigit = 0; //max displayTexts.Length
    int currentInput = 0;
    int currentCombination = 0;

    private void Start()
    {
        if (displayTexts.Length == 0)
        {
            Debug.LogWarning("No Digits assigned to rotary lock");
            lockEnabled = false;
            return;
        }
        joint = rotating.GetComponent<HingeJoint>();
    }

    public void OnGrab()
    {
        if (!lockEnabled) return;

        isTurning = true;
        if (currentDigit == 0)
        {
            foreach (TextMeshPro text in displayTexts)
            {
                text.text = "-";
            }
        }
    }

    public void OnRelease()
    {
        if (!lockEnabled) return;

        isTurning = false;
        currentCombination = currentCombination * 10 + currentInput;
        print(currentDigit);
        currentDigit++;
        if (currentDigit == displayTexts.Length)
        {
            if (currentCombination == combintion)
            {
                foreach (TextMeshPro text in displayTexts)
                {
                    text.color = correctColor;
                }
                ReleaseVault();
                lockEnabled = false;
                return;
            }
            else
            {
                currentDigit = 0;
                currentCombination = 0;
            }
        }

    }

    private void Update()
    {
        if (!lockEnabled) return;
        if (!isTurning) return;

        //\operatorname{mod}\left(\operatorname{round}\left(\frac{\left(x\ \ -\ \left(\frac{36}{2}\right)\right)}{36}\right)\ +3,\ 10\right)
        int number = modToInt(Mathf.RoundToInt((joint.angle - (18)) / 36) + 3, 10);

        if (number != currentInput)
        {
            currentInput = number;
            displayTexts[currentDigit].text = currentInput.ToString();
            //you could play a sound here
        }
    }

    void ReleaseVault()
    {
        rotating.GetComponent<XRGrabInteractable>().enabled = false;
        door.GetComponent<XRGrabInteractable>().enabled = true;
        door.GetComponent<Rigidbody>().isKinematic = false;

        //diamond is disabled because otherwise a player could poke their hand through and just grab it before the vault unlocks
        if (diamond != null)
        {
            diamond.GetComponent<XRGrabInteractable>().enabled = true;
            diamond.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    //using this mod because desmos handles negative numbers better in this way instead of using %
    int modToInt(float a, float b)
    {
        return Mathf.RoundToInt(a - b * Mathf.Floor(a / b));
    }
}
