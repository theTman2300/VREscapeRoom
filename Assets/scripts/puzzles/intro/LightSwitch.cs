using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint))]
public class LightSwitch : MonoBehaviour
{
    [SerializeField] UnityEvent onSwitchOn = new();

    HingeJoint joint;
    bool wasToggledOn = false;

    void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    void Update()
    {
        float angle = transform.eulerAngles.z;
        JointSpring spring = joint.spring;

        if (angle > 90)
        {
            spring.targetPosition = joint.limits.max;
        }
        else
        {
            spring.targetPosition = joint.limits.min;
            if (!wasToggledOn)
            {
                onSwitchOn.Invoke();
                wasToggledOn = true;
            }
        }
        joint.spring = spring;
    }
}
