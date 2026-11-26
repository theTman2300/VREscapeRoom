using System.Collections.Generic;
using System.Drawing;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(LineRenderer))]
public class Wire : MonoBehaviour
{
    //[SerializeField] int[] modifyLasers;
    [SerializeField] float maxToolToPointDistance = 0.1f;
    [SerializeField] float wireGap = 0.05f;
    [Header("Events")]
    [SerializeField] UnityEvent OnCableSnip = new();
    [SerializeField] UnityEvent OnWireRepair = new();

    bool wireActive = false;
    GameObject currentClippers;
    GameObject currentRepairTool;

    bool isCut = false;
    Vector3 cutPosition = Vector3.zero;

    LineRenderer lr1;
    LineRenderer lr2;
    LineRenderer lr3;

    private void Start()
    {
        lr1 = GetComponent<LineRenderer>();
        lr2 = transform.Find("Wire2Object").GetComponent<LineRenderer>();
        lr3 = transform.Find("Wire3Object").GetComponent<LineRenderer>();
        lr2.enabled = false;
        lr3.enabled = false;
    }

    void OnClipperUse(ActivateEventArgs eventArgs)
    {
        if (!wireActive || isCut) return;
        float closestDistance = Mathf.Infinity;
        Vector3 closestPosition = default;
        int closestPositionIndex = 0;

        for (int i = 0; i < lr1.positionCount; i++)
        {
            Vector3 point = lr1.GetPosition(i);
            float distance = Vector3.Distance(currentClippers.transform.position, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = point;
                closestPositionIndex = i;
            }
        }
        if (closestDistance > maxToolToPointDistance || closestPositionIndex == 0 || closestPositionIndex == lr1.positionCount - 1)
        {
            return;
        }

        isCut = true;
        OnCableSnip.Invoke();
        cutPosition = closestPosition;

        List<Vector3> lr2Positions = new();
        for (int i = 0; i < closestPositionIndex; i++)
        {
            lr2Positions.Add(lr1.GetPosition(i));
        }
        lr2Positions.Add(lr1.GetPosition(closestPositionIndex) - (lr1.GetPosition(closestPositionIndex) - lr1.GetPosition(closestPositionIndex - 1)).normalized * wireGap);

        List<Vector3> lr3Positions = new();
        lr3Positions.Add(lr1.GetPosition(closestPositionIndex) - (lr1.GetPosition(closestPositionIndex) - lr1.GetPosition(closestPositionIndex + 1)).normalized * wireGap);
        for (int i = closestPositionIndex + 1; i < lr1.positionCount; i++)
        {
            lr3Positions.Add(lr1.GetPosition(i));
        }

        lr2.positionCount = lr2Positions.Count;
        lr2.SetPositions(lr2Positions.ToArray());
        lr3.positionCount = lr3Positions.Count;
        lr3.SetPositions(lr3Positions.ToArray());
        lr1.enabled = false;
        lr2.enabled = true;
        lr3.enabled = true;
    }

    void OnRepairToolUse(ActivateEventArgs eventArgs)
    {
        if (!wireActive || !isCut) return;
        if (Vector3.Distance(currentRepairTool.transform.position, cutPosition) > maxToolToPointDistance) return;

        isCut = false;
        OnWireRepair.Invoke();

        lr1.enabled = true;
        lr2.enabled = false;
        lr3.enabled = false;
    }

    public void SetWireActive(bool state)
    {
        wireActive = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!wireActive) return;
        if (other.CompareTag("Clippers"))
        {
            other.gameObject.GetComponent<XRGrabInteractable>().activated.AddListener(OnClipperUse);
            currentClippers = other.gameObject;
        }
        if (other.CompareTag("WireRepairTool"))
        {
            other.gameObject.GetComponent<XRGrabInteractable>().activated.AddListener(OnRepairToolUse);
            currentRepairTool = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!wireActive) return;
        if (other.CompareTag("Clippers"))
        {
            other.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnClipperUse);
            currentClippers = null;
        }
        if (other.CompareTag("WireRepairTool"))
        {
            other.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnRepairToolUse);
            currentRepairTool = null;
        }
    }

    private void OnDisable()
    {
        if (currentClippers != null)
        {
            currentClippers.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnClipperUse);
        }
        if (currentRepairTool != null)
        {
            currentRepairTool.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnRepairToolUse);
        }
    }
}
