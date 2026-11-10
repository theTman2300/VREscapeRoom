using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    [HideInInspector] public Vector3 previousOffsetFromPortal;
    [SerializeField] public GameObject graphics;
    public bool hasSliceGraphics = false;
    [HideInInspector] public Material[] materials;
    [HideInInspector] public GameObject cloneGraphics;
    [HideInInspector] public Material[] cloneMaterials;
    public bool isHeld = false;
    [HideInInspector] public bool wasHeld = false;
    public bool isPlayer = false;
    [HideInInspector] public int portalEnterSide;

    bool shouldShowCloneGraphics = false;

    public virtual void Start()
    {
        materials = GetMaterials(graphics);

        if (graphics == null)
        {
            if (hasSliceGraphics)
                Debug.LogWarning("No graphics assigned for portalTraveller: " + name);
        }
        else
        {
            cloneGraphics = Instantiate(graphics, transform);
            cloneGraphics.SetActive(false);
        }

        cloneMaterials = GetMaterials(cloneGraphics);
    }

    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rotation)
    {
        transform.position = pos;
        transform.rotation = rotation;
    }

    public virtual void EnterPortalThreshold()
    {
        if (graphics != null)
        {
            shouldShowCloneGraphics = true;
            if (!graphics.activeSelf)
            {
                return;
            }
            cloneGraphics.gameObject.SetActive(true);
        }
    }

    public virtual void ExitPortalThreshold()
    {
        if (graphics != null)
        {
            shouldShowCloneGraphics = false;
            if (!graphics.activeSelf)
            {
                return;
            }
            cloneGraphics.gameObject.SetActive(false);
        }
    }

    Material[] GetMaterials(GameObject g)
    {
        List<MeshRenderer> renderers = new();
        List<SkinnedMeshRenderer> skinnedRenderers = new();
        List<Material> materialList = new();

        if (g == null)
        {
            Debug.LogWarning("No (clone)graphics assigned for portalTraveller: " + name);
            return materialList.ToArray();
        }

        renderers.AddRange(g.GetComponentsInChildren<MeshRenderer>());
        skinnedRenderers.AddRange(g.GetComponentsInChildren<SkinnedMeshRenderer>());

        foreach (MeshRenderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                materialList.Add(material);
            }
        }

        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                materialList.Add(material);
            }
        }

        return materialList.ToArray();
    }

    void OnGraphicsToggle(bool state)
    {
        if (!state)
        {
            cloneGraphics.gameObject.SetActive(false);
            return;
        }

        if (graphics != null)
            cloneGraphics.gameObject.SetActive(shouldShowCloneGraphics);
    }

    public void SetIsHeld(bool state)
    {
        isHeld = state;

        if (!state)
            wasHeld = true;
    }

    private void OnEnable()
    {
        if (!hasSliceGraphics)
            return;

        if (graphics.TryGetComponent(out ObjectEnableDisableEvent eventScript))
        {
            eventScript.OnObjectEnableDisable += OnGraphicsToggle;
        }
        else
        {
            graphics.AddComponent<ObjectEnableDisableEvent>().OnObjectEnableDisable += OnGraphicsToggle;
        }
    }

    private void OnDisable()
    {
        if (!hasSliceGraphics)
            return;

        if (graphics.TryGetComponent(out ObjectEnableDisableEvent eventScript))
        {
            eventScript.OnObjectEnableDisable -= OnGraphicsToggle;
        }
    }
}
