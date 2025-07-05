using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DescriptionLoader : MonoBehaviour
{
    private string descriptionsPath;

    void Start()
    {
        descriptionsPath = Path.Combine(Application.dataPath, "Preset/OriginalDescriptions");
        Debug.Log($"?? Description Loader Started. Path: {descriptionsPath}");
        LoadDescriptions();
    }

    void LoadDescriptions()
    {
        if (!Directory.Exists(descriptionsPath))
        {
            Debug.LogError($"? Descriptions folder NOT found: {descriptionsPath}");
            return;
        }

        Debug.Log($"? Descriptions folder FOUND: {descriptionsPath}");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"?? Found {allObjects.Length} objects in the scene.");

        foreach (GameObject obj in allObjects)
        {
            Debug.Log($"?? Checking object: {obj.name}");
        }

        foreach (GameObject obj in allObjects)
        {
            ObjectDescription objDesc = obj.GetComponent<ObjectDescription>();
            if (objDesc == null) continue; // Skip objects without ObjectDescription component

            string objectName = obj.name.ToLower().Replace(".l", "").Replace(".r", ""); // Remove .l and .r

            // Smartly replace numbers only when necessary
            Dictionary<string, string> numberWords = new Dictionary<string, string>()
{
    { "1st ", "first " }, { "2d ", "second " }, { "3rd ", "third " },
    { "4th ", "fourth " }, { "5th ", "fifth " }
};

            foreach (var pair in numberWords)
            {
                // Only replace if the number is a separate word (not inside another word like "1st finger")
                if (objectName.Contains(pair.Key) && !objectName.Contains("finger") && !objectName.Contains("toe"))
                {
                    objectName = objectName.Replace(pair.Key, pair.Value);
                }
            }

            string cleanName = objectName.Trim(); // Final cleaned name

            string descriptionFile = Path.Combine(descriptionsPath, cleanName + ".txt");



            if (File.Exists(descriptionFile))
            {
                string description = File.ReadAllText(descriptionFile);
                objDesc.descriptionText = description;
                Debug.Log($"? Loaded description for {obj.name}: {description}");
            }
            else
            {
                Debug.LogWarning($"? No description file found for {obj.name} (Expected: {cleanName}.txt)");
            }
        }
    }
}
