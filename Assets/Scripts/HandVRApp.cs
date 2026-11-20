using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace HandVR
{
    /// <summary>
    /// Builds the first HandVR interface at runtime. This is a functional UI/camera
    /// foundation; it does not perform ML hand recognition yet.
    /// </summary>
    public class HandVRApp : MonoBehaviour
    {
        private WebCamTexture webcam;
        private RawImage cameraImage;
        private Text status;
        private Text cameraButtonLabel;
        private bool cameraRunning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindObjectOfType<HandVRApp>() != null) return;
            new GameObject("HandVR App").AddComponent<HandVRApp>();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            BuildInterface();
            SetStatus("Pronto. A câmera ainda não foi iniciada.");
        }

        private void OnDestroy()
        {
            StopCamera();
        }

        private void BuildInterface()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem",
                    typeof(EventSystem), typeof(StandaloneInputModule));
            }

            var canvasObject = new GameObject("HandVR Canvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            var background = MakePanel(canvas.transform, "Background",
                new Color32(8, 12, 22, 255), Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero);

            MakeText(background.transform, "HANDVR", 54, FontStyle.Bold,
                new Color32(115, 224, 255, 255),
                new Vector2(0.08f, 0.91f), new Vector2(0.92f, 0.98f),
                TextAnchor.MiddleLeft);
            MakeText(background.transform, "CONTROLE DE REALIDADE VIRTUAL",
                22, FontStyle.Normal, new Color32(174, 190, 210, 255),
                new Vector2(0.08f, 0.875f), new Vector2(0.92f, 0.92f),
                TextAnchor.MiddleLeft);

            var previewPanel = MakePanel(background.transform, "CameraCard",
                new Color32(19, 27, 43, 255),
                new Vector2(0.07f, 0.48f), new Vector2(0.93f, 0.83f),
                Vector2.zero, Vector2.zero);
            MakeText(previewPanel.transform, "VISÃO DA CÂMERA", 24,
                FontStyle.Bold, Color.white, new Vector2(0.05f, 0.84f),
                new Vector2(0.95f, 0.98f), TextAnchor.MiddleLeft);

            var preview = new GameObject("CameraPreview",
                typeof(RectTransform), typeof(RawImage));
            preview.transform.SetParent(previewPanel.transform, false);
            var previewRect = preview.GetComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0.04f, 0.06f);
            previewRect.anchorMax = new Vector2(0.96f, 0.82f);
            previewRect.offsetMin = Vector2.zero;
            previewRect.offsetMax = Vector2.zero;
            cameraImage = preview.GetComponent<RawImage>();
            cameraImage.color = new Color32(30, 42, 60, 255);
            MakeText(previewPanel.transform,
                "A prévia aparece aqui quando você iniciar a câmera.",
                20, FontStyle.Normal, new Color32(175, 190, 210, 255),
                new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.57f),
                TextAnchor.MiddleCenter);

            var statusPanel = MakePanel(background.transform, "StatusCard",
                new Color32(19, 27, 43, 255),
                new Vector2(0.07f, 0.37f), new Vector2(0.93f, 0.455f),
                Vector2.zero, Vector2.zero);
            status = MakeText(statusPanel.transform, "Status", 22,
                FontStyle.Normal, new Color32(130, 225, 190, 255),
                new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f),
                TextAnchor.MiddleLeft);

            MakeText(background.transform, "MODO DE USO", 23, FontStyle.Bold,
                Color.white, new Vector2(0.08f, 0.30f),
                new Vector2(0.92f, 0.35f), TextAnchor.MiddleLeft);

            MakeButton(background.transform, "Câmera do celular",
                new Vector2(0.07f, 0.22f), new Vector2(0.48f, 0.285f),
                new Color32(24, 105, 139, 255), StartCamera);
            MakeButton(background.transform, "Parar câmera",
                new Vector2(0.52f, 0.22f), new Vector2(0.93f, 0.285f),
                new Color32(68, 78, 96, 255), StopCamera);

            MakeButton(background.transform, "Modo celular",
                new Vector2(0.07f, 0.135f), new Vector2(0.48f, 0.20f),
                new Color32(42, 76, 112, 255), () =>
                    SetStatus("Modo celular selecionado. Arraste na tela para olhar."));
            MakeButton(background.transform, "Modo VR Box",
                new Vector2(0.52f, 0.135f), new Vector2(0.93f, 0.20f),
                new Color32(91, 64, 145, 255), () =>
                    SetStatus("VR Box: use um segundo dispositivo ou câmera externa para ver as mãos."));

            MakeText(background.transform,
                "SEM GIROSCÓPIO: use toque/arrasto para olhar. " +
                "O rastreamento de mãos real será conectado em uma próxima etapa.",
                18, FontStyle.Normal, new Color32(150, 166, 188, 255),
                new Vector2(0.08f, 0.035f), new Vector2(0.92f, 0.115f),
                TextAnchor.MiddleCenter);
        }

        private void StartCamera()
        {
            if (cameraRunning) return;
            StartCoroutine(StartCameraRoutine());
        }

        private IEnumerator StartCameraRoutine()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                float wait = 0f;
                while (!Permission.HasUserAuthorizedPermission(Permission.Camera) && wait < 15f)
                {
                    wait += Time.unscaledDeltaTime;
                    yield return null;
                }
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    SetStatus("Permissão da câmera não concedida.");
                    yield break;
                }
            }
#endif
            if (WebCamTexture.devices.Length == 0)
            {
                SetStatus("Nenhuma câmera encontrada neste aparelho.");
                yield break;
            }

            WebCamDevice selected = WebCamTexture.devices[0];
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                if (!WebCamTexture.devices[i].isFrontFacing)
                {
                    selected = WebCamTexture.devices[i];
                    break;
                }
            }

            webcam = new WebCamTexture(selected.name, 1280, 720, 30);
            cameraImage.texture = webcam;
            webcam.Play();

            float timeout = 0f;
            while (webcam.width <= 16 && timeout < 10f)
            {
                timeout += Time.unscaledDeltaTime;
                yield return null;
            }

            cameraRunning = webcam.width > 16;
            SetStatus(cameraRunning
                ? "Câmera ativa. Prévia funcionando; mãos ainda não são reconhecidas."
                : "A câmera não iniciou. Verifique a permissão e tente novamente.");
        }

        private void StopCamera()
        {
            if (webcam != null)
            {
                if (webcam.isPlaying) webcam.Stop();
                Destroy(webcam);
                webcam = null;
            }
            cameraRunning = false;
            if (cameraImage != null) cameraImage.texture = null;
        }

        private void SetStatus(string message)
        {
            if (status != null) status.text = message;
            Debug.Log("[HandVR] " + message);
        }

        private static GameObject MakePanel(Transform parent, string name,
            Color color, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text MakeText(Transform parent, string value, int size,
            FontStyle style, Color color, Vector2 anchorMin,
            Vector2 anchorMax, TextAnchor alignment)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(8, 4);
            rect.offsetMax = new Vector2(-8, -4);
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = value;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static void MakeButton(Transform parent, string label,
            Vector2 anchorMin, Vector2 anchorMax, Color color,
            UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(label, typeof(RectTransform),
                typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(action);
            MakeText(go.transform, label, 21, FontStyle.Bold,
                Color.white, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
        }
    }
}
