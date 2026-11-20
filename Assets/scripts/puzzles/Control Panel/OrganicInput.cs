using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OrganicInput : MonoBehaviour
{
    [SerializeField] string correctObjectTag;
    [SerializeField] LayerMask objectLayerMask;
    [Space]
    [SerializeField] Transform floatPoint;
    [SerializeField] float floatPointMaxVerticalChange; //amplitude
    [SerializeField] float floatPointVerticalSpeed; //period
    [Space]
    [SerializeField] float repellVelocityMultiplier;
    [SerializeField, Range(0, 1)] float repellVerticalAngleClamp;
    [SerializeField] float floatSpeedMulitplier;

    GameObject currentObject;
    Rigidbody currentRigidbody;
    List<GameObject> gameObjectsInTrigger = new();

    [Button]
    public bool CheckCorrectObject()
    {
        Debug.Log("Test correct object: " + currentObject.CompareTag(correctObjectTag));
        return currentObject.CompareTag(correctObjectTag);
    }

    private void Update()
    {
        if (gameObjectsInTrigger.Count == 0) return;

        //float current object
        Vector3 floatGoal = floatPoint.position + floatPoint.up * (Mathf.Sin(Time.time * floatPointVerticalSpeed) * floatPointMaxVerticalChange);
        currentRigidbody.linearVelocity = (floatGoal - currentObject.transform.position) * floatSpeedMulitplier;

        //repell other objects
        foreach (GameObject repellingObject in gameObjectsInTrigger)
        {
            if (repellingObject == currentObject) continue;
            Vector3 direction = (repellingObject.transform.position - currentObject.transform.position).normalized;
            direction = new(Mathf.Clamp(direction.x, -repellVerticalAngleClamp / 2, repellVerticalAngleClamp / 2), direction.y, Mathf.Clamp(direction.z, -repellVerticalAngleClamp / 2, repellVerticalAngleClamp / 2));
            repellingObject.GetComponent<Rigidbody>().linearVelocity = (repellingObject.transform.position - currentObject.transform.position).normalized * repellVelocityMultiplier;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectLayerMask != (objectLayerMask | (1 << other.gameObject.layer))) return;
        if (gameObjectsInTrigger.Contains(other.gameObject)) return;

        gameObjectsInTrigger.Add(other.gameObject);
        other.gameObject.GetComponent<Rigidbody>().useGravity = false;

        currentObject = other.gameObject;
        currentRigidbody = other.gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectLayerMask != (objectLayerMask | (1 << other.gameObject.layer))) return;

        gameObjectsInTrigger.Remove(other.gameObject);
        other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        other.gameObject.GetComponent<SetGravityOnRelease>().SetUsedGravity(true);
        if (other.gameObject == currentObject)
        {
            if (gameObjectsInTrigger.Count != 0)
            {
                currentObject = gameObjectsInTrigger[0];
                currentRigidbody = currentObject.GetComponent<Rigidbody>();
            }
            else
            {
                currentObject = null;
                currentRigidbody = null;
            }
        }
    }
}
