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
    [SerializeField] float maxClipperToPointDistance = 0.1f;
    [SerializeField] float snipGap = 0.05f;
    [SerializeField] UnityEvent OnCableSnip = new();

    bool clippersNearby = false;
    bool canSnip = false;
    GameObject currentClippers;

    LineRenderer lr1;
    LineRenderer lr2;

    private void Start()
    {
        lr1 = GetComponent<LineRenderer>();
        lr2 = transform.Find("Wire2Object").GetComponent<LineRenderer>();
    }

    void OnClipperUse(ActivateEventArgs eventArgs)
    {
        if (!canSnip) return;
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
        if (closestDistance > maxClipperToPointDistance || closestPositionIndex == 0 || closestPositionIndex == lr1.positionCount - 1)
        {
            return;
        }

        canSnip = false;
        OnCableSnip.Invoke();

        List<Vector3> lr1Positions = new();
        for (int i = 0; i < closestPositionIndex; i++)
        {
            lr1Positions.Add(lr1.GetPosition(i));
        }
        lr1Positions.Add(lr1.GetPosition(closestPositionIndex) - (lr1.GetPosition(closestPositionIndex) - lr1.GetPosition(closestPositionIndex - 1)).normalized * snipGap);

        List<Vector3> lr2Positions = new();
        lr2Positions.Add(lr1.GetPosition(closestPositionIndex) - (lr1.GetPosition(closestPositionIndex) - lr1.GetPosition(closestPositionIndex + 1)).normalized * snipGap);
        for (int i = closestPositionIndex + 1; i < lr1.positionCount; i++)
        {
            lr2Positions.Add(lr1.GetPosition(i));
        }

        lr1.positionCount = lr1Positions.Count;
        lr1.SetPositions(lr1Positions.ToArray());
        lr2.positionCount = lr2Positions.Count;
        lr2.SetPositions(lr2Positions.ToArray());
    }

    public void ActiveWire()
    {
        canSnip = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canSnip) return;
        if (!other.CompareTag("Clippers")) return;
        other.gameObject.GetComponent<XRGrabInteractable>().activated.AddListener(OnClipperUse);
        currentClippers = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canSnip) return;
        if (!other.CompareTag("Clippers")) return;
        other.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnClipperUse);
        currentClippers = null;
    }

    private void OnDisable()
    {
        if (currentClippers != null)
        {
            currentClippers.gameObject.GetComponent<XRGrabInteractable>().activated.RemoveListener(OnClipperUse);
        }
    }
}
