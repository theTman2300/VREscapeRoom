using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    [HideInInspector] public Vector3 previousOffsetFromPortal;
    [SerializeField] public GameObject graphics;
    public bool hasSliceGraphics = false;
    [HideInInspector] public Material[] materials;
    [HideInInspector] public GameObject cloneGraphics;
    [HideInInspector] public Material[] cloneMaterials;

    public virtual void Start()
    {
        materials = GetMaterials(graphics);

        if (graphics == null)
            Debug.LogWarning("No graphics assigned for portalTraveller: " + name);
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
            cloneGraphics.gameObject.SetActive(true);
    }

    public virtual void ExitPortalThreshold()
    {
        if (graphics != null)
            cloneGraphics.gameObject.SetActive(false);
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
}
