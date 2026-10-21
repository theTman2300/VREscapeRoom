using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalScript : MonoBehaviour
{
    [SerializeField] PortalScript linkedPortal;
    [SerializeField] Camera playerCamera;
    public MeshRenderer screen1;
    public MeshRenderer screen2;
    [Tooltip("How many portals deep you can see when portals are facing each other")]
    [SerializeField, Min(0)] int portalDepth = 0;
    [Tooltip("Resolution of the portal will be (screen / this)")]
    [SerializeField, Min(1)] float portalQuality = 1;
    [SerializeField] float screenDistanceMargin = 0.07f;
    Camera portalCamera;
    List<Camera> portalDepthCameras;
    RenderTexture portalTexture;

    List<PortalTraveller> trackedTravellers;

    void Start()
    {
        portalCamera = GetComponentInChildren<Camera>();
        //playerCamera = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>();
        portalCamera.enabled = false;
        trackedTravellers = new();
        portalDepthCameras = new();
        CreatePortalDepthCameras();
    }

    void CreatePortalDepthCameras()
    {
        for (int i = 0; i < portalDepth; i++)
        {
            portalDepthCameras.Add(Instantiate(portalCamera, transform));
        }
    }

    void SetScreensOffset()
    {

        //https://docs.unity3d.com/6000.6/Documentation/Manual/FrustumSizeAtDistance.html
        //not sure how much the frustum stuff is doing or if it is mostly just the margin i put that does most the work
        //still cool and only runs once per portal so im keeping it >:)
        var frustumHeight = 2.0f * playerCamera.nearClipPlane * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * playerCamera.aspect;
        float cornerDistance = new Vector3(frustumWidth, frustumHeight, playerCamera.nearClipPlane).magnitude;

        float offset = cornerDistance * 1.5f + screenDistanceMargin;
        int side = Math.Sign(SideOfPortal(playerCamera.transform.position));
        screen1.transform.localPosition = new Vector3(0, 0, offset - (offset * side));
        screen2.transform.localPosition = new Vector3(0, 0, -offset - (offset * side));
    }

    void AvoidClipping()
    {
        if (SideOfPortal(playerCamera.transform.position) > 0)
        {
            screen1.transform.localRotation = Quaternion.Euler(0, 180, 0);
            screen2.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            screen1.transform.localRotation = Quaternion.Euler(0, 0, 0);
            screen2.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void AvoidClipping(Vector3 pos)
    {
        if (SideOfPortal(pos) > 0)
        {
            screen1.transform.localRotation = Quaternion.Euler(0, 180, 0);
            screen2.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            screen1.transform.localRotation = Quaternion.Euler(0, 0, 0);
            screen2.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void CreatePortalTexture()
    {
        if (portalTexture == null || portalTexture.width != Screen.width || portalTexture.height != Screen.height)
        {
            if (portalTexture != null)
            {
                portalTexture.Release();
            }

            portalTexture = new RenderTexture(Mathf.RoundToInt(Screen.width / portalQuality), Mathf.RoundToInt(Screen.height / portalQuality), 24);
            portalTexture.name = name + " portalTexture";
            portalCamera.targetTexture = portalTexture;
            foreach (Camera camera in portalDepthCameras)
            {
                camera.targetTexture = portalTexture;
            }

            linkedPortal.screen1.material.SetTexture("_MainTex", portalTexture);
            linkedPortal.screen2.material.SetTexture("_MainTex", portalTexture);
        }
    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    private void Update()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerTransform = traveller.transform;

            Vector3 offsetFromPortal = travellerTransform.position - transform.position;
            int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            if (portalSide != portalSideOld)
            {
                Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerTransform.localToWorldMatrix;
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);

                linkedPortal.OnTravellerEnterPortal(traveller);
                linkedPortal.AvoidClipping(playerCamera.transform.position);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    void UpdateSlice(PortalTraveller traveller)
    {
        if (!traveller.hasSliceGraphics) return;

        Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.graphics.transform.localToWorldMatrix;
        traveller.cloneGraphics.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        int side = Math.Sign(SideOfPortal(traveller.transform.position));
        Vector3 sliceNormal = transform.forward * side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * -side;

        for (int i = 0; i < traveller.materials.Length; i++)
        {
            traveller.materials[i].SetVector("_sliceCenter", transform.position);
            traveller.materials[i].SetVector("_sliceNormal", sliceNormal);

            traveller.cloneMaterials[i].SetVector("_sliceCenter", linkedPortal.transform.position);
            traveller.cloneMaterials[i].SetVector("_sliceNormal", cloneSliceNormal);
        }
    }

    void SetFallback(bool useFallback)
    {
        float useIt = useFallback ? 1 : 0;
        linkedPortal.screen1.material.SetFloat("_UseFallback", useIt);
        linkedPortal.screen2.material.SetFloat("_UseFallback", useIt);
    }

    //implements oblique cliping as explained here https://www.youtube.com/watch?v=cWpFZbjtSQg at 13:14
    void SetNearClipPlane(Camera camera)
    {
        Transform clipPlane = transform;
        int dot = Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - camera.transform.position));

        Vector3 cameraSpacePos = camera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 cameraSpaceNormal = camera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float cameraSpaceDistance = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, cameraSpaceDistance);

        camera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    private void Render(ScriptableRenderContext context, Camera camera)
    {
        if (playerCamera == null || camera != playerCamera) return;

        for (int i = 0; i < trackedTravellers.Count; i++)
            UpdateSlice(trackedTravellers[i]);

        AvoidClipping();
        SetScreensOffset();

        if (!VisibleFromCamera(linkedPortal.screen1, playerCamera)) return;

        screen1.enabled = false;
        screen2.enabled = false;
        CreatePortalTexture();

        //----set camera positions
        // with (linkedPortal.transform.worldToLocalMatrix * playerCamera.localToWorldMatrix) you get a relative position,
        // and then by multiplying that with (transform.localToWorldMatrix) you place it back to world space but relative to this portal
        Matrix4x4 m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCamera.transform.localToWorldMatrix;
        portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        SetNearClipPlane(portalCamera);
        for (int i = 0; i < portalDepthCameras.Count; i++)
        {
            Camera previousCamera = i == 0 ? portalCamera : portalDepthCameras[i - 1];
            Matrix4x4 mDepth = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * previousCamera.transform.localToWorldMatrix;
            portalDepthCameras[i].transform.SetPositionAndRotation(mDepth.GetColumn(3), mDepth.rotation);
            SetNearClipPlane(portalDepthCameras[i]);
        }

        //----render camera's
        linkedPortal.AvoidClipping(portalCamera.transform.position);
        int visibleDepth = portalDepth;
        for (int i = portalDepthCameras.Count - 1; i >= 0; i--)
        {
            if (!VisibleFromCamera(linkedPortal.screen1, portalDepthCameras[i]))
                visibleDepth--;
        }
        bool deepestCamera = true;
        //render in backwards order for correct effect
        for (int i = portalDepth - 1; i >= 0; i--)
        {
            //if statement instead of changing for loop for debugging purposes
            if (i > visibleDepth)
            {
                //Debug.Log("Skipped at depth: " + (i + 1) + "   " + name);
                continue;
            }
            SetFallback(deepestCamera);
            deepestCamera = false;
            UniversalRenderPipeline.SubmitRenderRequest(portalDepthCameras[i], new UniversalRenderPipeline.SingleCameraRequest());
        }

        SetFallback(false);
        UniversalRenderPipeline.SubmitRenderRequest(portalCamera, new UniversalRenderPipeline.SingleCameraRequest());
        linkedPortal.AvoidClipping(playerCamera.transform.position);

        screen1.enabled = true;
        screen2.enabled = true;
    }

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    float SideOfPortal(Vector3 pos)
    {
        return Vector3.Dot(pos - transform.position, transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PortalTraveller traveller))
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PortalTraveller traveller) && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);

            if (traveller.hasSliceGraphics)
            {
                for (int i = 0; i < traveller.materials.Length; i++)
                {
                    traveller.materials[i].SetVector("_sliceCenter", new Vector3(-1000000, 0, 0));
                    traveller.materials[i].SetVector("_sliceNormal", new Vector3(1, 0, 0));

                    traveller.cloneMaterials[i].SetVector("_sliceCenter", new Vector3(-1000000, 0, 0));
                    traveller.cloneMaterials[i].SetVector("_sliceNormal", new Vector3(1, 0, 0));
                }
            }
        }
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += Render;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= Render;
    }
}
