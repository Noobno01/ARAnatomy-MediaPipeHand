using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetAngularVelocityOnRelease : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (rb.isKinematic)
        {
            rb.angularVelocity = Vector3.zero; // Prevent angular velocity errors
        }
    }
}
