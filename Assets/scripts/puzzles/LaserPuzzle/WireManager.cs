using UnityEngine;
using UnityEngine.Events;

public class WireManager : MonoBehaviour
{
    [SerializeField] GameObject[] lasers;
    [SerializeField] UnityEvent OnLasersDisable = new();

    bool[] laserStates;
    bool wirePuzzleActive = true;

    private void Start()
    {
        laserStates = new bool[lasers.Length];
        for (int i = 0; i < laserStates.Length; i++)
        {
            laserStates[i] = true;
        }

    }

    public void OnWireChange(int[] modifyLasers)
    {
        if (!wirePuzzleActive) return;
        if (modifyLasers.Length == 0)
        {
            Debug.Log("A wire had no assigned modifyLasers");
            return;
        }

        foreach (int i in modifyLasers)
        {
            laserStates[i - 1] = !laserStates[i - 1];
            lasers[i - 1].SetActive(laserStates[i - 1]);
        }

        if (!IsLaserwallActive())
        {
            OnLasersDisable.Invoke();
            wirePuzzleActive = false;
        }
    }

    bool IsLaserwallActive()
    {
        foreach (bool laserState in laserStates)
        {
            if (laserState) return true;
        }
        return false;
    }
}
