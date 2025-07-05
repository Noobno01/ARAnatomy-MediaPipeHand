using UnityEngine;

public class PlainNameDescription : MonoBehaviour
{
    void Start()
    {
        SetPlainDescriptions();
    }

    void SetPlainDescriptions()
    {
        // Get all ObjectDescription components in children (deep search)
        ObjectDescription[] allDescriptions = GetComponentsInChildren<ObjectDescription>(true);

        foreach (ObjectDescription objDesc in allDescriptions)
        {
            if (objDesc == null) continue;

            // Get the GameObject name and clean it
            string plainName = objDesc.gameObject.name
                .Replace(".l", "")
                .Replace(".r", "")
                .Replace(".", "")
                .Trim();

            // Set the plain name as description
            objDesc.descriptionText = plainName;

            Debug.Log($"?? Set description for {objDesc.gameObject.name}: {plainName}");
        }
    }
}
