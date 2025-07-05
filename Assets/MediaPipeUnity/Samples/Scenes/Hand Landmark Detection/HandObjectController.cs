using UnityEngine;
using Mediapipe.Tasks.Vision.HandLandmarker;
namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    internal class HandObjectController : MonoBehaviour
    {
        public Transform objectToMove;  // Assign this in Unity Inspector
        private HandLandmarkerResult latestHandResult;
        // Inside HandObjectController
        


        void Update()
        {
            if (latestHandResult.handLandmarks.Count > 0)
            {
                var handLandmarks = latestHandResult.handLandmarks[0]; // First detected hand

                // Get the index finger tip landmark (ID 8)
                var indexFingerTip = handLandmarks.landmarks[8];

                // Convert normalized coordinates (0-1) to world space
                Vector3 newPosition = new Vector3(indexFingerTip.x * 10, indexFingerTip.y * 10, -indexFingerTip.z * 10);
                objectToMove.position = newPosition;
            }
        }

        public void UpdateHandData(HandLandmarkerResult result)
        {
            latestHandResult = result;
        }
    }
}