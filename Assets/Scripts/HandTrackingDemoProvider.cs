using System;
using UnityEngine;

namespace HandVR
{
    // Provedor FALSO para testar UI/fluxo sem câmera.
    // Não detecta uma mão real. Substituir por MediaPipe no app final.
    public class HandTrackingDemoProvider : MonoBehaviour, IHandTrackingProvider
    {
        public event Action<HandLandmarkFrame> OnFrame;
        public bool IsRunning { get; private set; }

        public void StartTracking()
        {
            IsRunning = true;
            Debug.Log("HandVR: provedor de demonstração iniciado; sem detecção real.");
        }

        public void StopTracking() => IsRunning = false;

        // O adaptador MediaPipe deverá chamar este método com 21 landmarks reais.
        public void SubmitFrame(HandLandmarkFrame frame)
        {
            if (IsRunning) OnFrame?.Invoke(frame);
        }

        private void OnDestroy() => StopTracking();
    }
}
