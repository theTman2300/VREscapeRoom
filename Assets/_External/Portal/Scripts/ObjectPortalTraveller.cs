using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectPortalTraveller : PortalTraveller
{
    Rigidbody rb;
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rotation)
    {
        base.Teleport(fromPortal, toPortal, pos, rotation);
        Quaternion rotationDifference = toPortal.rotation * Quaternion.Inverse(fromPortal.rotation);
        rb.linearVelocity = rotationDifference * rb.linearVelocity;
    }
}
