using UnityEngine;


    public class ObjectDescription : MonoBehaviour
    {
        [TextArea] // Allows for multi-line text in the Inspector
        [Tooltip("Description of the object")]
        public string descriptionText;

        public string DescriptionText => descriptionText;
    }
