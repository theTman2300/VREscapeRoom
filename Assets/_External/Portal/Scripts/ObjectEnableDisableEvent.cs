using System;
using UnityEngine;

public class ObjectEnableDisableEvent : MonoBehaviour
{
    public event Action<bool> OnObjectEnableDisable;

    private void OnEnable()
    {
        OnObjectEnableDisable?.Invoke(true);
    }

    private void OnDisable()
    {
        OnObjectEnableDisable?.Invoke(false);
    }
}
