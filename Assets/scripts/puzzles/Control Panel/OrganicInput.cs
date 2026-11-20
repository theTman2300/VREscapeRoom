using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class OrganicInput : MonoBehaviour
{
    [SerializeField] string correctObjectTag;
    [SerializeField] LayerMask objectLayerMask;
    [Space]
    [SerializeField] Transform floatPoint;
    [SerializeField] float floatPointMaxVerticalChange; //amplitude
    [SerializeField] float floatPointVerticalSpeed; //period
    [Space]
    [SerializeField] float repellVelocity;
    [SerializeField] float repellAngleDegreesRange;
    [SerializeField] float floatSpeedMulitplier;

    GameObject currentObject;
    Rigidbody currentRigidbody;
    List<GameObject> gameObjectsInTrigger;

    public bool CheckCorrectObject()
    {
        return currentObject.CompareTag(correctObjectTag);
    }

    private void Update()
    {
        if (currentObject == null) return;

        Vector3 floatGoal = floatPoint.position + floatPoint.up * (Mathf.Sin(Time.time * floatPointVerticalSpeed) * floatPointVerticalSpeed);
        float speed = Vector3.Distance(currentObject.transform.position, floatGoal) * floatSpeedMulitplier;
        currentRigidbody.linearVelocity = (floatGoal - currentObject.transform.position).normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectLayerMask != (objectLayerMask | (1 << other.gameObject.layer))) return;

        gameObjectsInTrigger.Add(other.gameObject);
        currentObject = other.gameObject;
        other.gameObject.GetComponent<Rigidbody>().useGravity = false;

        if (currentObject == null)
        {
            currentObject = other.gameObject;
            currentRigidbody = other.gameObject.GetComponent<Rigidbody>();
            return;
        }

        Quaternion repellDirection = Quaternion.Euler(new(Random.Range(-repellAngleDegreesRange / 2, repellAngleDegreesRange / 2), Random.Range(-180, 180), 0));
        Vector3 repellForce = repellDirection * (floatPoint.forward * repellVelocity);
        currentObject.GetComponent<Rigidbody>().linearVelocity = repellForce;

        currentRigidbody = other.gameObject.GetComponent<Rigidbody>();
        currentObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectLayerMask != (objectLayerMask | (1 << other.gameObject.layer))) return;

        gameObjectsInTrigger.Remove(other.gameObject);
        other.gameObject.GetComponent<Rigidbody>().useGravity = true;
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
