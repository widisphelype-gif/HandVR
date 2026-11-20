using System;
using UnityEngine;

namespace HandVR
{
    /// <summary>
    /// Android MediaPipe Tasks Vision adapter. Requires the hand_landmarker.task
    /// model in the Android library assets folder described in SETUP.md.
    /// </summary>
    public class MediaPipeHandTrackingProvider : MonoBehaviour, IHandTrackingProvider
    {
        public event Action<HandLandmarkFrame> OnFrame;
        public bool IsRunning { get; private set; }

        private WebCamTexture source;
        private Color32[] pixels;
        private int[] argb;
        private float nextFrameTime;
        private long lastTimestamp;
#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaClass bridge;
#endif

        public void StartTracking(WebCamTexture camera)
        {
            if (camera == null || !camera.isPlaying)
            {
                Debug.LogWarning("[HandVR] Start the camera before hand tracking.");
                return;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                bridge = new AndroidJavaClass("com.handvr.mediapipe.HandLandmarkerBridge");
                bool initialized = bridge.CallStatic<bool>("initialize");
                if (!initialized)
                {
                    Debug.LogError("[HandVR] MediaPipe could not initialize. Check the model asset.");
                    return;
                }
                source = camera;
                IsRunning = true;
                Debug.Log("[HandVR] MediaPipe hand tracking started.");
            }
            catch (Exception exception)
            {
                Debug.LogError("[HandVR] Native MediaPipe bridge unavailable: " + exception);
            }
#else
            Debug.LogWarning("[HandVR] Real MediaPipe tracking is available only in an Android build.");
#endif
        }

        public void StartTracking()
        {
            Debug.LogWarning("[HandVR] Use StartTracking(WebCamTexture) after starting the camera.");
        }

        private void Update()
        {
            if (!IsRunning || source == null || !source.isPlaying ||
                Time.unscaledTime < nextFrameTime) return;

            nextFrameTime = Time.unscaledTime + 0.10f; // target ~10 inference frames/s
            try
            {
                if (pixels == null || pixels.Length != source.width * source.height)
                {
                    pixels = new Color32[source.width * source.height];
                    argb = new int[pixels.Length];
                }
                source.GetPixels32(pixels);
                for (int i = 0; i < pixels.Length; i++)
                {
                    Color32 p = pixels[i];
                    argb[i] = (p.a << 24) | (p.r << 16) | (p.g << 8) | p.b;
                }

                long timestamp = (long)(Time.realtimeSinceStartup * 1000.0);
                if (timestamp <= lastTimestamp) timestamp = lastTimestamp + 1;
                lastTimestamp = timestamp;
#if UNITY_ANDROID && !UNITY_EDITOR
                float[] values = bridge.CallStatic<float[]>("detect",
                    argb, source.width, source.height, timestamp);
                if (values != null && values.Length >= 63)
                {
                    var landmarks = new HandLandmark[21];
                    for (int i = 0; i < 21; i++)
                        landmarks[i] = new HandLandmark(values[i * 3],
                            values[i * 3 + 1], values[i * 3 + 2]);
                    OnFrame?.Invoke(new HandLandmarkFrame {
                        hasHand = true, landmarks = landmarks,
                        timestamp = Time.realtimeSinceStartup
                    });
                }
                else
                {
                    OnFrame?.Invoke(new HandLandmarkFrame {
                        hasHand = false, landmarks = Array.Empty<HandLandmark>(),
                        timestamp = Time.realtimeSinceStartup
                    });
                }
#endif
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[HandVR] Frame inference failed: " + exception.Message);
            }
        }

        public void StopTracking()
        {
            IsRunning = false;
            source = null;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (bridge != null)
            {
                bridge.CallStatic("close");
                bridge.Dispose();
                bridge = null;
            }
#endif
        }

        private void OnDestroy() => StopTracking();
    }
}
