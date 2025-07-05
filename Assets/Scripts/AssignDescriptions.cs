using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AsssignDescription : MonoBehaviour
{
    private Dictionary<string, string> descriptions;

    [Tooltip("Path to the JSON file relative to Resources folder, e.g., MuscleDescriptions")]
    public string jsonFileName = "MuscleDescriptions"; // No .json extension needed

    void Start()
    {
        LoadDescriptionsFromJSON();
        AssignDescriptionsToChildren();
    }

    void LoadDescriptionsFromJSON()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError($"? JSON file '{jsonFileName}.json' not found in Resources folder!");
            return;
        }

        descriptions = JsonUtility.FromJson<DictionaryWrapper>(jsonFile.text).items;

        Debug.Log($"? Loaded {descriptions.Count} descriptions from JSON.");
    }

    void AssignDescriptionsToChildren()
    {
        foreach (Transform child in transform)
        {
            ObjectDescription objDesc = child.GetComponent<ObjectDescription>();
            if (objDesc == null) continue;

            string key = child.name;
            if (descriptions.ContainsKey(key))
            {
                objDesc.descriptionText = descriptions[key];
                Debug.Log($"?? Assigned description to {key}");
            }
            else
            {
                Debug.LogWarning($"?? No description found for {key} in JSON.");
            }
        }
    }

    [System.Serializable]
    public class DescriptionDictionary
    {
        public Dictionary<string, string> items;
    }
    [System.Serializable]
    public class DictionaryWrapper
    {
        public Dictionary<string, string> items;
    }

}
