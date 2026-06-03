using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MantaMinigames.Fishing
{
    // Construye una UI simple para probar pesca sin depender de escenas principales.
    [ExecuteAlways]
    public sealed class FishingTestSceneBuilder : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private FishingMinigameController minigameController;
        [SerializeField] private FishingTestSceneStarter starter;

        private const string PanelName = "FishingMinigamePanel";
        private const string BarName = "PrecisionBar";
        private const string SuccessName = "SuccessZone";
        private const string IndicatorName = "MovingIndicator";
        private const string AttemptsName = "AttemptsText";
        private const string MessageName = "MessageText";
        private const string ButtonName = "StartButton";

        private void Awake()
        {
            BuildIfNeeded();
        }

        private void OnEnable()
        {
            BuildIfNeeded();
        }

        [ContextMenu("Build Fishing Test UI")]
        public void BuildIfNeeded()
        {
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
            }

            if (canvas == null)
            {
                return;
            }

            EnsureEventSystem();

            RectTransform panel = GetOrCreateRect(canvas.transform, PanelName, new Vector2(520f, 260f));
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            Image panelImage = EnsureImage(panel.gameObject, new Color(0.07f, 0.09f, 0.11f, 0.92f));

            RectTransform bar = GetOrCreateRect(panel, BarName, new Vector2(420f, 32f));
            bar.anchoredPosition = new Vector2(0f, 32f);
            Image barImage = EnsureImage(bar.gameObject, new Color(0.18f, 0.2f, 0.24f, 1f));

            RectTransform successZone = GetOrCreateRect(bar, SuccessName, new Vector2(90f, 32f));
            successZone.anchoredPosition = Vector2.zero;
            Image successImage = EnsureImage(successZone.gameObject, new Color(0.16f, 0.72f, 0.32f, 1f));

            RectTransform indicator = GetOrCreateRect(bar, IndicatorName, new Vector2(12f, 52f));
            indicator.anchoredPosition = Vector2.zero;
            Image indicatorImage = EnsureImage(indicator.gameObject, new Color(1f, 0.92f, 0.32f, 1f));

            TextMeshProUGUI attemptsText = GetOrCreateText(panel, AttemptsName, "Intentos: 3", 24f, new Vector2(0f, 94f), new Vector2(420f, 36f));
            TextMeshProUGUI messageText = GetOrCreateText(panel, MessageName, "Presiona Iniciar o R. Luego usa Space, Enter o clic.", 21f, new Vector2(0f, -34f), new Vector2(460f, 48f));
            Button startButton = GetOrCreateButton(panel, ButtonName, "Iniciar", new Vector2(0f, -104f), new Vector2(180f, 42f));

            minigameController = panel.GetComponent<FishingMinigameController>();
            if (minigameController == null)
            {
                minigameController = panel.gameObject.AddComponent<FishingMinigameController>();
            }

            starter = panel.GetComponent<FishingTestSceneStarter>();
            if (starter == null)
            {
                starter = panel.gameObject.AddComponent<FishingTestSceneStarter>();
            }

            minigameController.SetUiReferences(barImage, successImage, indicatorImage, messageText, attemptsText);

            if (Application.isPlaying)
            {
                startButton.onClick.RemoveListener(starter.StartFishingTest);
                startButton.onClick.AddListener(starter.StartFishingTest);
            }

            _ = panelImage;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static RectTransform GetOrCreateRect(Transform parent, string objectName, Vector2 size)
        {
            Transform existing = parent.Find(objectName);
            GameObject target = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
            target.transform.SetParent(parent, false);

            RectTransform rect = (RectTransform)target.transform;
            rect.localScale = Vector3.one;
            rect.sizeDelta = size;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            return rect;
        }

        private static Image EnsureImage(GameObject target, Color color)
        {
            Image image = target.GetComponent<Image>();
            if (image == null)
            {
                image = target.AddComponent<Image>();
            }

            image.color = color;
            return image;
        }

        private static TextMeshProUGUI GetOrCreateText(RectTransform parent, string objectName, string text, float size, Vector2 position, Vector2 rectSize)
        {
            RectTransform rect = GetOrCreateRect(parent, objectName, rectSize);
            rect.anchoredPosition = position;

            TextMeshProUGUI label = rect.GetComponent<TextMeshProUGUI>();
            if (label == null)
            {
                label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            }

            label.text = text;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;
            return label;
        }

        private static Button GetOrCreateButton(RectTransform parent, string objectName, string labelText, Vector2 position, Vector2 rectSize)
        {
            RectTransform buttonRect = GetOrCreateRect(parent, objectName, rectSize);
            buttonRect.anchoredPosition = position;
            Image buttonImage = EnsureImage(buttonRect.gameObject, new Color(0.18f, 0.38f, 0.72f, 1f));

            Button button = buttonRect.GetComponent<Button>();
            if (button == null)
            {
                button = buttonRect.gameObject.AddComponent<Button>();
            }

            button.targetGraphic = buttonImage;

            TextMeshProUGUI label = GetOrCreateText(buttonRect, "Label", labelText, 22f, Vector2.zero, rectSize);
            label.raycastTarget = false;
            return button;
        }
    }
}
