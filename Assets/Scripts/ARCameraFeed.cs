using System;
using Mediapipe;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public sealed class ARCameraFeed : MonoBehaviour
{
    [SerializeField] ARCameraManager cameraManager;
    [SerializeField] HandLandmarkerRunner runner;
    [SerializeField] RawImage screen;
    [SerializeField] bool rotateCounterClockwise90 = true;

    private Mediapipe.Unity.Screen screenComponent;
    Texture2D tex;
    NativeArray<byte> buffer;

    void Awake()
    {
        if (screen == null)
        {
            Debug.LogError("RawImage screen reference is not assigned in the Inspector.");
            return;
        }

        screenComponent = screen.GetComponent<Mediapipe.Unity.Screen>();
        if (screenComponent == null)
        {
            screenComponent = screen.gameObject.AddComponent<Mediapipe.Unity.Screen>();
        }

        screenComponent.texture = null;
    }

    void OnEnable() => cameraManager.frameReceived += OnFrame;
    void OnDisable()
    {
        cameraManager.frameReceived -= OnFrame;
        if (buffer.IsCreated) buffer.Dispose();
    }

    unsafe void OnFrame(ARCameraFrameEventArgs _)
    {
        Debug.Log("AR frame received");
        if (!cameraManager.TryAcquireLatestCpuImage(out var img)) return;

        using (img)
        {
            int w = img.width, h = img.height, bytes = w * h * 4;
            if (!buffer.IsCreated || buffer.Length != bytes)
            {
                if (buffer.IsCreated) buffer.Dispose();
                buffer = new NativeArray<byte>(bytes, Allocator.Persistent);

                if (tex != null) Destroy(tex);
                tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

                // Set the texture on our screen
                screenComponent.texture = tex;

                // Apply rotation - if we want to rotate 90 degrees counterclockwise
                if (rotateCounterClockwise90)
                {
                    // Rotate the screen component using MediaPipe's RotationAngle enum
                    // We're rotating 90° counterclockwise, which corresponds to Rotation270
                    screenComponent.Rotate(RotationAngle.Rotation270);

                    // Swap width and height for the screen size
                    screenComponent.Resize(h, w);
                }
                else
                {
                    // Normal resize without rotation
                    screenComponent.Resize(w, h);
                }
            }

            var conv = new XRCpuImage.ConversionParams(
                         img, TextureFormat.RGBA32,
                         XRCpuImage.Transformation.MirrorX);

            img.Convert(conv,
                (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr(buffer),
                buffer.Length);

            tex.LoadRawTextureData(buffer);
            tex.Apply(false);

            using var mpImg = new Mediapipe.Image(ImageFormat.Types.Format.Srgba,
                                        w, h, w * 4, buffer);

            if (!runner.annotationReady)
            {
                runner.PrepareAnnotations(img.width, img.height);
            }

            runner.ProcessARImage(mpImg, (long)(Time.realtimeSinceStartup * 1000));
        }
    }
}