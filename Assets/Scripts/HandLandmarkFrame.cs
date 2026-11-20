using System;
using UnityEngine;

namespace HandVR
{
    [Serializable]
    public struct HandLandmark
    {
        // Coordenadas normalizadas da imagem: x/y entre 0 e 1.
        public float x;
        public float y;
        public float z;

        public HandLandmark(float x, float y, float z = 0f)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }

    [Serializable]
    public class HandLandmarkFrame
    {
        public bool hasHand;
        public HandLandmark[] landmarks;
        public float timestamp;
    }

    public enum HandGesture
    {
        None,
        Pinch,
        OpenPalm,
        Fist
    }

    public interface IHandTrackingProvider
    {
        event Action<HandLandmarkFrame> OnFrame;
        bool IsRunning { get; }
        void StartTracking();
        void StopTracking();
    }
}
