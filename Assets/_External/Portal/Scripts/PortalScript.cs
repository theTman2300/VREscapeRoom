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
    //[Tooltip("How many portals deep you can see when portals are facing each other")]
    //[SerializeField, Min(0)] int portalDepth = 0;
    [Tooltip("Resolution of the portal will be (screen / this)")]
    [SerializeField, Min(1)] float portalQuality = 1;
    [SerializeField] float screenDistanceMargin = 0.07f;
    [SerializeField] float nearClipLimit = 0.05f;
    Camera portalCamera;
    List<Camera> portalDepthCameras;
    RenderTexture[] eyeTextures = new RenderTexture[2];

    List<PortalTraveller> trackedTravellers;

    void Start()
    {
        portalCamera = GetComponentInChildren<Camera>();
        //playerCamera = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>();
        portalCamera.enabled = false;
        trackedTravellers = new();
        portalDepthCameras = new();
        //CreatePortalDepthCameras();
    }

    //void CreatePortalDepthCameras()
    //{
    //    for (int i = 0; i < portalDepth; i++)
    //    {
    //        portalDepthCameras.Add(Instantiate(portalCamera, transform));
    //    }
    //}

    void SetScreensOffset()
    {
        //old stuff from before vr
        ////https://docs.unity3d.com/6000.6/Documentation/Manual/FrustumSizeAtDistance.html
        ////not sure how much the frustum stuff is doing or if it is mostly just the margin i put that does most the work
        ////still cool and only runs once per portal so im keeping it >:)
        //var frustumHeight = 2.0f * playerCamera.nearClipPlane * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //var frustumWidth = frustumHeight * playerCamera.aspect;
        //float cornerDistance = new Vector3(frustumWidth, frustumHeight, playerCamera.nearClipPlane).magnitude;

        //float offset = cornerDistance * 1.5f + screenDistanceMargin;
        float offset = screenDistanceMargin;
        int side = Math.Sign(SideOfPortal(GetHeadPosition()));
        screen1.transform.localPosition = new Vector3(0, 0, offset - (offset * side));
        screen2.transform.localPosition = new Vector3(0, 0, -offset - (offset * side));
    }

    public void AvoidClipping()
    {
        if (SideOfPortal(GetHeadPosition()) > 0)
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
        //get width/height of eye and not screen
        int eyeWidth = Mathf.RoundToInt(UnityEngine.XR.XRSettings.eyeTextureWidth / portalQuality);
        int eyeHeight = Mathf.RoundToInt(UnityEngine.XR.XRSettings.eyeTextureHeight / portalQuality);

        bool changedResolution = false;
        for (int i = 0; i < 2; i++)
        {
            RenderTexture currentEyeTexture = eyeTextures[i];
            if (currentEyeTexture != null && currentEyeTexture.width == eyeWidth && currentEyeTexture.height == eyeHeight) continue;
            if (currentEyeTexture != null ) { currentEyeTexture.Release(); Destroy(currentEyeTexture); }
            eyeTextures[i] = new RenderTexture(eyeWidth, eyeHeight, 24);
            changedResolution = true;
        }

        if (!changedResolution) return;
        foreach (var screen in new[] { linkedPortal.screen1, linkedPortal.screen2 })
        {
            screen.material.SetTexture("_LeftEyeTex", eyeTextures[0]);
            screen.material.SetTexture("_RightEyeTex", eyeTextures[1]);
        }
    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    static bool VisibleFromCamera(Renderer renderer, Matrix4x4 view, Matrix4x4 projection)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(projection * view);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
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
                linkedPortal.AvoidClipping();
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

    //void SetFallback(bool useFallback)
    //{
    //    float useIt = useFallback ? 1 : 0;
    //    linkedPortal.screen1.material.SetFloat("_UseFallback", useIt);
    //    linkedPortal.screen2.material.SetFloat("_UseFallback", useIt);
    //}

    //implements oblique cliping as explained here https://www.youtube.com/watch?v=cWpFZbjtSQg at 13:14
    void SetNearClipPlane(Camera camera, int dot)
    {
        Transform clipPlane = transform;
        float camSideDistance = Vector3.Dot(clipPlane.forward, camera.transform.position - clipPlane.position); //gets signed distance
        bool onCorrectSide = Math.Sign(camSideDistance) == -dot; //check the side of the portal of the eye compared to the camera
        if (!onCorrectSide || Mathf.Abs(camSideDistance) < nearClipLimit)
            return;

        Vector3 cameraSpacePos = camera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 cameraSpaceNormal = camera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float cameraSpaceDistance = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal);
        Vector4 clipPlaneCameraSpace = new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, cameraSpaceDistance);

        camera.projectionMatrix = camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    private void RenderVR()
    {
        screen1.enabled = false;
        screen2.enabled = false;
        CreatePortalTexture();

        int clipSide = linkedPortal.SideOfPortal(GetHeadPosition()) >= 0 ? -1 : 1;

        for (int i = 0; i < 2; i++)
        {
            Camera.StereoscopicEye eye = (Camera.StereoscopicEye)i; //convert int to eye
            Matrix4x4 view = playerCamera.GetStereoViewMatrix(eye);
            Matrix4x4 projection = playerCamera.GetStereoProjectionMatrix(eye);
            
            if (!VisibleFromCamera(linkedPortal.screen1, view, projection)) continue;

            Matrix4x4 eyeToWorld = view.inverse * Matrix4x4.Scale(new Vector3(1, 1, -1)); //basically playerCamera.transform.localToWorldMatrix. Invert to go view to world instead of the default world to view and scale for correct handedness
            Matrix4x4 m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * eyeToWorld;

            portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
            portalCamera.targetTexture = eyeTextures[i];

            //set portalcamera to eye projection so SetNearClipPlane() sets it for the eye and not the screen
            portalCamera.projectionMatrix = projection;
            SetNearClipPlane(portalCamera, clipSide);

            linkedPortal.AvoidClipping();
            UniversalRenderPipeline.SubmitRenderRequest(portalCamera, new UniversalRenderPipeline.SingleCameraRequest());
        }

        linkedPortal.AvoidClipping();
        screen1.enabled = true;
        screen2.enabled = true;
    }

    private void Render(ScriptableRenderContext context, Camera camera)
    {
        if (playerCamera == null || camera != playerCamera) return;

        for (int i = 0; i < trackedTravellers.Count; i++)
            UpdateSlice(trackedTravellers[i]);

        AvoidClipping();
        SetScreensOffset();

        if (playerCamera.stereoEnabled)
        {
            RenderVR();
        }
        else return;

        //if (!VisibleFromCamera(linkedPortal.screen1, playerCamera)) return;

        //screen1.enabled = false;
        //screen2.enabled = false;
        //CreatePortalTexture();

        ////----set camera positions
        //// with (linkedPortal.transform.worldToLocalMatrix * playerCamera.localToWorldMatrix) you get a relative position,
        //// and then by multiplying that with (transform.localToWorldMatrix) you place it back to world space but relative to this portal
        //Matrix4x4 m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCamera.transform.localToWorldMatrix;
        //portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        //SetNearClipPlane(portalCamera);
        ////for (int i = 0; i < portalDepthCameras.Count; i++)
        ////{
        ////    Camera previousCamera = i == 0 ? portalCamera : portalDepthCameras[i - 1];
        ////    Matrix4x4 mDepth = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * previousCamera.transform.localToWorldMatrix;
        ////    portalDepthCameras[i].transform.SetPositionAndRotation(mDepth.GetColumn(3), mDepth.rotation);
        ////    SetNearClipPlane(portalDepthCameras[i]);
        ////}

        ////----render camera's
        //linkedPortal.AvoidClipping(portalCamera.transform.position);
        ////int visibleDepth = portalDepth;
        ////for (int i = portalDepthCameras.Count - 1; i >= 0; i--)
        ////{
        ////    if (!VisibleFromCamera(linkedPortal.screen1, portalDepthCameras[i]))
        ////        visibleDepth--;
        ////}
        ////bool deepestCamera = true;
        //////render in backwards order for correct effect
        ////for (int i = portalDepth - 1; i >= 0; i--)
        ////{
        ////    //if statement instead of changing for loop for debugging purposes
        ////    if (i > visibleDepth)
        ////    {
        ////        //Debug.Log("Skipped at depth: " + (i + 1) + "   " + name);
        ////        continue;
        ////    }
        ////    SetFallback(deepestCamera);
        ////    deepestCamera = false;
        ////    UniversalRenderPipeline.SubmitRenderRequest(portalDepthCameras[i], new UniversalRenderPipeline.SingleCameraRequest());
        ////}

        //SetFallback(false);
        //UniversalRenderPipeline.SubmitRenderRequest(portalCamera, new UniversalRenderPipeline.SingleCameraRequest());
        //linkedPortal.AvoidClipping(playerCamera.transform.position);

        //screen1.enabled = true;
        //screen2.enabled = true;
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

    Vector3 GetHeadPosition()
    {
        Vector3 l = playerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse.GetColumn(3);
        Vector3 r = playerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse.GetColumn(3);
        return (l + r) * 0.5f;
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
