using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveObject : PortalTraveller
{
    [SerializeField] float speed;
    [SerializeField] bool isMoving = true;

    Rigidbody rb;
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) isMoving = !isMoving;

        Vector3 movement = speed * transform.forward;
        movement *= isMoving ? 1 : 0;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rotation)
    {
        base.Teleport(fromPortal, toPortal, pos, rotation);
        Quaternion rotationDifference = toPortal.rotation * Quaternion.Inverse(fromPortal.rotation);
        rb.linearVelocity = rotationDifference * rb.linearVelocity;
    }
}
