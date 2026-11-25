using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SatelliteLever : MonoBehaviour
{
    [SerializeField] ConfigurableJoint leverJoint;
    [SerializeField] int maxValue;
    [Space]
    [SerializeField] TextMeshPro maxValueText;
    [SerializeField] TextMeshPro displayText;
    [Space]
    [SerializeField] UnityEvent OnNumberChange;

    int lastSelectedNumber;
    float jointLimit;
    Vector3 topPosition;

    private void Start()
    {
        maxValueText.text = string.Format(maxValueText.text, maxValue);
        jointLimit = leverJoint.linearLimit.limit;
        topPosition = leverJoint.transform.forward * jointLimit + leverJoint.transform.position;
        lastSelectedNumber = maxValue / 2;
        displayText.text = lastSelectedNumber.ToString();
    }

    void Update()
    {
        float t = 1 - (Vector3.Distance(leverJoint.transform.position, topPosition) / (jointLimit * 2));
        int selectedNumber = Mathf.RoundToInt(t * maxValue);

        if (selectedNumber != lastSelectedNumber)
        {
            OnNumberChange.Invoke();
            displayText.text = selectedNumber.ToString();
            lastSelectedNumber = selectedNumber;
        }
    }

    public int GetCurrentInput()
    {
        return lastSelectedNumber;
    }
}
