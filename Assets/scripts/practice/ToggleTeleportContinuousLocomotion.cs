using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ToggleTeleportContinuousLocomotion : MonoBehaviour
{
    [SerializeField] TeleportationProvider teleportationLocomotion;
    [SerializeField] ContinuousMoveProvider continuousLocomotion;
    [SerializeField] XRRayInteractor[] teleportInteractors;

    public void SetTeleportLocomotion(bool setTeleport)
    {
        if (setTeleport)
        {
            SetTeleport();
        }
        else
        {
            SetContinuous();
        }
    }

    public void SetTeleport()
    {
        teleportationLocomotion.enabled = true;
        continuousLocomotion.enabled = false;
        foreach (XRRayInteractor interactor in teleportInteractors)
        {
            interactor.enabled = true;
        }
    }

    public void SetContinuous()
    {
        teleportationLocomotion.enabled = false;
        continuousLocomotion.enabled = true;
        foreach (XRRayInteractor interactor in teleportInteractors)
        {
            interactor.enabled = false;
        }
    }
}
