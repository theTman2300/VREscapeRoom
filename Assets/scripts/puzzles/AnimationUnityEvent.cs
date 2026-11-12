using UnityEngine;
using UnityEngine.Events;

public class AnimationUnityEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnTrigger = new();

    public void OnAnimationTrigger()
    {
        OnTrigger.Invoke();
    }
}
