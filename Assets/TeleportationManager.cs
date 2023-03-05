using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] InputActionAsset actionAsset;
    [SerializeField] XRRayInteractor rayInteractor;
    InputAction thumbstick;
    [SerializeField] TeleportationProvider teleportationProvider;
    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;
        var activate = actionAsset.FindActionMap("XRI LeftHand Locomotion", true).FindAction("Teleport Mode Activate", true);
        activate.Enable();
        activate.performed += OnTeleportActivate;

        var cancel = actionAsset.FindActionMap("XRI LeftHand Locomotion", true).FindAction("Teleport Mode Cancel", true);
        cancel.Enable();
        cancel.performed += OnTeleportCanceled;

        thumbstick = actionAsset.FindActionMap("XRI LeftHand Locomotion", true).FindAction("RobAction", true);
        thumbstick.Enable();
    }

    void OnTeleportActivate(InputAction.CallbackContext ctx)
    {
        rayInteractor.enabled = true;
    }

    void OnTeleportCanceled(InputAction.CallbackContext ctx)
    {
        rayInteractor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rayInteractor.enabled) return;

        if (thumbstick.ReadValue<Vector2>() != Vector2.zero) return;

        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit target))
        {
            rayInteractor.enabled = false;
            return;
        }

        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = target.point,
        };

        teleportationProvider.QueueTeleportRequest(request);
        rayInteractor.enabled = false;
    }
}
