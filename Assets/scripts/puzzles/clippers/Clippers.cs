using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Clippers : MonoBehaviour
{
    [SerializeField] float clipAreaRadius = .1f;
    [SerializeField] Vector3 clipAreaOffset = Vector3.zero;
    [Space]
    [SerializeField] UnityEvent OnUseClippers = new();

    public void UseClippers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + transform.rotation * clipAreaOffset, clipAreaRadius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Leaf")) continue;
            GameObject leaf = collider.gameObject;
            leaf.GetComponent<XRGrabInteractable>().enabled = true;
            leaf.GetComponent<Rigidbody>().isKinematic = false;
            OnUseClippers.Invoke();
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.rotation * clipAreaOffset, clipAreaRadius);
    }
}
