using UnityEngine;

public class SetXRotationTo90 : MonoBehaviour
{
    void Start()
    {
        // Get all parent and child transforms
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allTransforms)
        {
            // Get the current rotation
            Vector3 currentRotation = t.eulerAngles;

            // Set the X-axis to 90 degrees
            t.rotation = Quaternion.Euler(90, currentRotation.y, currentRotation.z);
        }
    }
}
