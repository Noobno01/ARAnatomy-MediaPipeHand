using UnityEngine;
using System.IO;

public class ExportGameObjectNames : MonoBehaviour
{
    void Start()
    {
        // Save to Unity project root folder
        string projectRoot = Application.dataPath.Replace("/Assets", "");
        string filePath = Path.Combine(projectRoot, "GameObjectNames.txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                writer.WriteLine(child.name);
                count++;
            }
            Debug.Log($"? Exported {count} GameObject names to: {filePath}");
        }
    }
}
