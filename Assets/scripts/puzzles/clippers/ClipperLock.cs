using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ClipperLock : MonoBehaviour
{
    [SerializeField] GameObject key;
    [SerializeField] GameObject keyBase;
    [SerializeField] XRSocketInteractor socketInteractor;
    [SerializeField] GameObject clippers;

    public void KeyInserted()
    {
        StartCoroutine(StartLockAnimation());
    }

    IEnumerator StartLockAnimation()
    {
        yield return new WaitForSeconds(socketInteractor.recycleDelayTime);
        key.GetComponent<XRGrabInteractable>().enabled = false;
        key.GetComponent<Rigidbody>().isKinematic = true;
        key.transform.parent = keyBase.transform.Find("KeyPosition");
        key.transform.localPosition = Vector3.zero;
        key.transform.localRotation = Quaternion.identity;

        keyBase.GetComponent<Animator>().SetTrigger("Open");
    }

    public void OnLockAnimation()
    {
        //release clippers here
        clippers.GetComponent<XRGrabInteractable>().enabled = true;
        clippers.GetComponent<Rigidbody>().isKinematic = false;
    }
}
