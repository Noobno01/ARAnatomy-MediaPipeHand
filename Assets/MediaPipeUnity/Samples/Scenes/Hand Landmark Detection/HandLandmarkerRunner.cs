// Copyright (c) 2023 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    public class HandLandmarkerRunner : VisionTaskApiRunner<HandLandmarker>
    {
        [SerializeField] private HandLandmarkerResultAnnotationController _handLandmarkerResultAnnotationController;

        public bool annotationReady = false;

        public readonly HandLandmarkDetectionConfig config = new HandLandmarkDetectionConfig();

        protected override IEnumerator Run()
        {
            yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            var options = config.GetHandLandmarkerOptions(OnHandLandmarkDetectionOutput);

            var canUseGpuImage = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 &&
                                 GpuManager.GpuResources != null;
            using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

            taskApi = HandLandmarker.CreateFromOptions(options, GpuManager.GpuResources);
            yield break;
        }

        public void ProcessARImage(Mediapipe.Image image, long timestampMs)
        {
            switch (taskApi.runningMode)
            {
                case Tasks.Vision.Core.RunningMode.IMAGE:
                    var res = taskApi.Detect(image);
                    _handLandmarkerResultAnnotationController.DrawNow(res);
                    break;

                case Tasks.Vision.Core.RunningMode.VIDEO:
                    var resV = taskApi.DetectForVideo(image, timestampMs);
                    _handLandmarkerResultAnnotationController.DrawNow(resV);
                    break;

                case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                    taskApi.DetectAsync(image, timestampMs);
                    break;
            }
        }

        private Mediapipe.Tasks.Components.Containers.NormalizedLandmarks latestHandLandmarks;
        private bool scaleRequested = false;
        private bool rotateRequested = false;

        public GameObject objectToScale;

        public void SetObjectToScale(string obj)
        {
            objectToScale = GameObject.Find(obj);
        }

        private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Mediapipe.Image image, long timestampMs)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _handLandmarkerResultAnnotationController.DrawNow(result);

                if (result.handLandmarks != null && result.handLandmarks.Count > 0)
                {
                    Debug.Log("?? Hand Detected - Timestamp: " + timestampMs);
                    latestHandLandmarks = result.handLandmarks[0];

                    if (IsPinchGesture(latestHandLandmarks))
                    {
                        scaleRequested = true;
                        Debug.Log("Pinch gesture detected - scaling");
                    }
                    else if (IsRotationGesture(latestHandLandmarks))
                    {
                        rotateRequested = true;
                        Debug.Log("Rotation gesture detected - rotating");
                    }
                }
                else
                {
                    Debug.Log("No Hand Detected");
                }
            });
        }

        private bool IsPinchGesture(NormalizedLandmarks handLandmarks)
        {
            var thumbTip = handLandmarks.landmarks[4];
            var indexTip = handLandmarks.landmarks[8];
            float pinchDistance = Vector2.Distance(
                new Vector2(thumbTip.x, thumbTip.y),
                new Vector2(indexTip.x, indexTip.y)
            );
            return pinchDistance < 0.15f;
        }

        private bool IsRotationGesture(NormalizedLandmarks handLandmarks)
        {
            var thumbTip = handLandmarks.landmarks[4];
            var indexTip = handLandmarks.landmarks[8];
            var middleTip = handLandmarks.landmarks[12];

            float thumbIndexDistance = Vector2.Distance(
                new Vector2(thumbTip.x, thumbTip.y),
                new Vector2(indexTip.x, indexTip.y)
            );

            float indexMiddleDistance = Vector2.Distance(
                new Vector2(indexTip.x, indexTip.y),
                new Vector2(middleTip.x, middleTip.y)
            );

            return thumbIndexDistance > 0.15f && indexMiddleDistance < 0.08f;
        }

        private void Update()
        {
            if (scaleRequested)
            {
                if (objectToScale == null)
                {
                    Debug.LogWarning("No object assigned for scaling.");
                    scaleRequested = false;
                    return;
                }
                ScaleObjectWithPinch(latestHandLandmarks);
                scaleRequested = false;
            }

            if (rotateRequested)
            {
                if (objectToScale == null)
                {
                    Debug.LogWarning("No object assigned for rotation.");
                    rotateRequested = false;
                    return;
                }
                RotateObjectWithIndexMiddle(latestHandLandmarks);
                rotateRequested = false;
            }
        }

        private Vector3 originalScale;
        private bool originalScaleSet = false;

        private void ScaleObjectWithPinch(NormalizedLandmarks handLandmarks)
        {
            if (!originalScaleSet)
            {
                originalScale = objectToScale.transform.localScale;
                originalScaleSet = true;
            }

            var thumbTip = handLandmarks.landmarks[4];
            var indexTip = handLandmarks.landmarks[8];
            float pinchDistance = Vector2.Distance(
                new Vector2(thumbTip.x, thumbTip.y),
                new Vector2(indexTip.x, indexTip.y)
            );

            float scaleFactor = pinchDistance * 20f;
            objectToScale.transform.localScale = originalScale * scaleFactor;
        }

        private Vector3 originalRotation;
        private bool originalRotationSet = false;
        private float previousIndexMiddleAngle;
        private bool previousAngleSet = false;

        private void RotateObjectWithIndexMiddle(NormalizedLandmarks handLandmarks)
        {
            if (!originalRotationSet)
            {
                originalRotation = objectToScale.transform.eulerAngles;
                originalRotationSet = true;
            }

            var indexTip = handLandmarks.landmarks[8];
            var middleTip = handLandmarks.landmarks[12];

            Vector2 indexPos = new Vector2(indexTip.x, indexTip.y);
            Vector2 middlePos = new Vector2(middleTip.x, middleTip.y);
            Vector2 direction = middlePos - indexPos;
            float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (previousAngleSet)
            {
                float angleDelta = Mathf.DeltaAngle(previousIndexMiddleAngle, currentAngle);
                float rotationAmount = angleDelta * 1.5f;

                Vector3 currentRotation = objectToScale.transform.eulerAngles;
                currentRotation.y += rotationAmount;
                objectToScale.transform.eulerAngles = currentRotation;
            }

            previousIndexMiddleAngle = currentAngle;
            previousAngleSet = true;
        }

        public void PrepareAnnotations(int width, int height)
        {
            _handLandmarkerResultAnnotationController.isMirrored = false;
            _handLandmarkerResultAnnotationController.imageSize = new Vector2Int(width, height);
            screen.Resize(width, height);
            annotationReady = true;
        }
    }
}
