using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WireManager : MonoBehaviour
{
    [SerializeField] GameObject[] lasers;
    [SerializeField] UnityEvent OnLasersDisable = new();
    [Space]
    [SerializeField] int wireAmount;
    [SerializeField] TextMeshPro countdownText;
    [SerializeField] int countdownSeconds;
    [SerializeField] Color failColor;
    [SerializeField] Color succesColor;

    Color defaultTextColor;

    bool wirePuzzleActive = true;
    bool isCountingDown = false;
    int cutWireCount = 0;
    float countdownTimer = 0;

    private void Start()
    {
        defaultTextColor = countdownText.color;
        countdownText.text = countdownSeconds.ToString();
    }

    public void OnWireCut()
    {
        if (!wirePuzzleActive) return;

        if (cutWireCount == 0 && !isCountingDown)
        {
            isCountingDown = true;
            countdownTimer = countdownSeconds;
        }

        cutWireCount++;

        if (isCountingDown && cutWireCount >= wireAmount)
        {
            DisableLaserwall();
        }
    }

    public void OnWireRepair()
    {
        if (!wirePuzzleActive) return;

        cutWireCount--;

        if (!isCountingDown && cutWireCount == 0)
        {
            countdownText.color = defaultTextColor;
            countdownText.text = countdownSeconds.ToString();
        }
    }

    private void Update()
    {
        if (!wirePuzzleActive) return;
        if (!isCountingDown) return;

        countdownTimer -= Time.deltaTime;
        countdownText.text = Mathf.Ceil(countdownTimer).ToString();
        if (countdownTimer <= 0)
        {
            isCountingDown = false;
            countdownText.color = failColor;
        }
    }

    void DisableLaserwall()
    {
        wirePuzzleActive = false;
        countdownText.color = succesColor;

        foreach (GameObject laser in lasers)
        {
            laser.SetActive(false);
        }
        OnLasersDisable.Invoke();
    }
}
