using UnityEngine;
using UnityEngine.Events;

public class WireManager : MonoBehaviour
{
    [SerializeField] int wireCutCountGoal = 4;
    [SerializeField] GameObject lasers;
    [SerializeField] UnityEvent OnLasersDisable = new();

    int cutWireCount = 0;

    public void OnWireCut()
    {
        cutWireCount++;

        if (cutWireCount == wireCutCountGoal)
        {
            OnLasersDisable.Invoke();
            lasers.SetActive(false);
        }
    }
}
