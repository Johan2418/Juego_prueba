using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MantaMinigames.Fishing
{
    // Arma la UI temporal de pesca para el mapa sin depender de misiones o NPCs.
    [ExecuteAlways]
    public sealed class FishingMapMinigameSceneBuilder : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private FishingMinigameLauncher fishingLauncher;
        [SerializeField] private FishingPlayerFishingState fishingState;

        private const string MinigamePanelName = "FishingMinigameUI";
        private const string BarName = "PrecisionBar";
        private const string SuccessName = "SuccessZone_Green";
        private const string IndicatorName = "MovingIndicator";
        private const string AttemptsName = "Text_Attempts";
        private const string MessageName = "Text_Message";
        private const string StateName = "Text_HasFish";
        private const string ResultName = "Text_FishingResult";

        private void Awake()
        {
            BuildIfNeeded();
        }

        private void OnEnable()
        {
            BuildIfNeeded();
        }

        [ContextMenu("Build Map Fishing Minigame UI")]
        public void BuildIfNeeded()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }

            if (canvas == null || fishingLauncher == null || fishingState == null)
            {
                return;
            }

            RectTransform panel = GetOrCreateRect(canvas.transform, MinigamePanelName, new Vector2(520f, 260f));
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            EnsureImage(panel.gameObject, new Color(0.07f, 0.09f, 0.11f, 0.92f));

            RectTransform bar = GetOrCreateRect(panel, BarName, new Vector2(420f, 32f));
            bar.anchoredPosition = new Vector2(0f, 32f);
            Image barImage = EnsureImage(bar.gameObject, new Color(0.18f, 0.2f, 0.24f, 1f));

            RectTransform successZone = GetOrCreateRect(bar, SuccessName, new Vector2(90f, 32f));
            Image successImage = EnsureImage(successZone.gameObject, new Color(0.16f, 0.72f, 0.32f, 1f));

            RectTransform indicator = GetOrCreateRect(bar, IndicatorName, new Vector2(12f, 52f));
            Image indicatorImage = EnsureImage(indicator.gameObject, new Color(1f, 0.92f, 0.32f, 1f));

            TextMeshProUGUI attemptsText = GetOrCreateText(panel, AttemptsName, "Intentos: 3", 24f, new Vector2(0f, 94f), new Vector2(420f, 36f));
            TextMeshProUGUI messageText = GetOrCreateText(panel, MessageName, "Interactua con el muelle para pescar.", 21f, new Vector2(0f, -34f), new Vector2(460f, 48f));
            TextMeshProUGUI stateText = GetOrCreateText(panel, StateName, "HasFish: False", 18f, new Vector2(-120f, -96f), new Vector2(210f, 30f));
            TextMeshProUGUI resultText = GetOrCreateText(panel, ResultName, "Resultado: None", 18f, new Vector2(120f, -96f), new Vector2(230f, 30f));

            FishingMinigameController controller = panel.GetComponent<FishingMinigameController>();
            if (controller == null)
            {
                controller = panel.gameObject.AddComponent<FishingMinigameController>();
            }

            controller.SetUiReferences(barImage, successImage, indicatorImage, messageText, attemptsText);
            fishingLauncher.Configure(controller, fishingState, stateText, resultText);
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
    }
}
