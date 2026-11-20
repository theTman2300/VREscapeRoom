using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetGravityOnRelease : MonoBehaviour
{
    bool shouldChangeGravity = false;
    bool gravityState;

    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetUsedGravity(bool state)
    {
        shouldChangeGravity = true;
        gravityState = state;
    }

    public void OnDeselect()
    {
        if (!shouldChangeGravity) return;
        rb.useGravity = gravityState;
    }
}
