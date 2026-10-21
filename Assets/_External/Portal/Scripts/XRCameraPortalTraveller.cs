using Unity.XR.CoreUtils;
using UnityEngine;

public class XRCameraPortalTraveller : PortalTraveller
{
    [SerializeField] Transform XROrigin;

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rotation)
    {
        //base.Teleport(fromPortal, toPortal, pos, rotation);
        Matrix4x4 m = toPortal.transform.localToWorldMatrix * fromPortal.worldToLocalMatrix * XROrigin.transform.localToWorldMatrix;
        XROrigin.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        transform.position = pos;
        transform.rotation = rotation;
    }
}
