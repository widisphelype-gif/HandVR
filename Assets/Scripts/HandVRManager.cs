using UnityEngine;
using UnityEngine.UI;

namespace HandVR
{
    public class HandVRManager : MonoBehaviour
    {
        [SerializeField] private HandTrackingDemoProvider provider;
        [SerializeField] private Text statusText;
        [SerializeField] private bool startOnLaunch = false;

        private void Start()
        {
            if (provider == null) provider = FindObjectOfType<HandTrackingDemoProvider>();
            if (provider != null) provider.OnFrame += HandleHandFrame;
            SetStatus("HandVR pronto • rastreador ainda não conectado");
            if (startOnLaunch) StartHands();
        }

        public void StartHands()
        {
            if (provider == null) { SetStatus("Provedor de mãos não configurado"); return; }
            provider.StartTracking();
            SetStatus("Modo de demonstração ativo • aguardando MediaPipe");
        }

        public void StopHands()
        {
            provider?.StopTracking();
            SetStatus("Rastreamento pausado");
        }

        private void HandleHandFrame(HandLandmarkFrame frame)
        {
            HandGesture gesture = HandGestureRecognizer.Recognize(frame);
            SetStatus(frame.hasHand ? "Mão: " + gesture : "Nenhuma mão detectada");
        }

        private void SetStatus(string value)
        {
            if (statusText != null) statusText.text = value;
            Debug.Log(value);
        }

        private void OnDestroy()
        {
            if (provider != null) provider.OnFrame -= HandleHandFrame;
        }
    }
}
