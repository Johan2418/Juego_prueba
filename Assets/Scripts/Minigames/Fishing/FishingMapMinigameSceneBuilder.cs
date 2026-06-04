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
        private const string TargetFishName = "Text_TargetFish";
        private const string AttemptsName = "Text_Attempts";
        private const string MessageName = "Text_Message";
        private const string HitsName = "Text_Hits";
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

        private void Start()
        {
            BuildIfNeeded();
        }

        public void ConfigureSceneReferences(Canvas targetCanvas, FishingMinigameLauncher targetLauncher, FishingPlayerFishingState targetState)
        {
            if (targetCanvas != null)
            {
                canvas = targetCanvas;
            }

            if (targetLauncher != null)
            {
                fishingLauncher = targetLauncher;
            }

            if (targetState != null)
            {
                fishingState = targetState;
            }
        }

        public FishingMinigameController BuildIfNeeded()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }

            if (canvas == null)
            {
                canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
            }

            if (fishingLauncher == null)
            {
                fishingLauncher = UnityEngine.Object.FindFirstObjectByType<FishingMinigameLauncher>();
            }

            if (fishingState == null)
            {
                fishingState = UnityEngine.Object.FindFirstObjectByType<FishingPlayerFishingState>();
            }

            if (fishingState == null)
            {
                GameObject stateObject = new GameObject(nameof(FishingPlayerFishingState));
                fishingState = stateObject.AddComponent<FishingPlayerFishingState>();
            }

            if (canvas == null || fishingLauncher == null || fishingState == null)
            {
                return null;
            }

            RectTransform panel = GetOrCreateRect(canvas.transform, MinigamePanelName, new Vector2(560f, 300f));
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.anchoredPosition = Vector2.zero;
            EnsureImage(panel.gameObject, new Color(0.07f, 0.09f, 0.11f, 0.92f));

            RectTransform bar = GetOrCreateRect(panel, BarName, new Vector2(420f, 32f));
            bar.anchoredPosition = new Vector2(0f, 24f);
            Image barImage = EnsureImage(bar.gameObject, new Color(0.18f, 0.2f, 0.24f, 1f));

            RectTransform successZone = GetOrCreateRect(bar, SuccessName, new Vector2(90f, 32f));
            Image successImage = EnsureImage(successZone.gameObject, new Color(0.16f, 0.72f, 0.32f, 1f));

            RectTransform indicator = GetOrCreateRect(bar, IndicatorName, new Vector2(12f, 52f));
            Image indicatorImage = EnsureImage(indicator.gameObject, new Color(1f, 0.92f, 0.32f, 1f));

            TextMeshProUGUI targetFishText = GetOrCreateText(panel, TargetFishName, "Pez: Pescado", 21f, new Vector2(0f, 122f), new Vector2(460f, 30f));
            TextMeshProUGUI attemptsText = GetOrCreateText(panel, AttemptsName, "Intentos: 3", 23f, new Vector2(-120f, 86f), new Vector2(220f, 32f));
            TextMeshProUGUI hitsText = GetOrCreateText(panel, HitsName, "Aciertos: 0/1", 23f, new Vector2(120f, 86f), new Vector2(220f, 32f));
            TextMeshProUGUI messageText = GetOrCreateText(panel, MessageName, "Interactua con el muelle para pescar.", 21f, new Vector2(0f, -40f), new Vector2(500f, 48f));
            TextMeshProUGUI stateText = GetOrCreateText(panel, StateName, "HasFish: False", 18f, new Vector2(-125f, -112f), new Vector2(220f, 30f));
            TextMeshProUGUI resultText = GetOrCreateText(panel, ResultName, "Resultado: None", 18f, new Vector2(130f, -112f), new Vector2(250f, 30f));

            if (panel.GetComponent<FishingInputHandler>() == null)
            {
                panel.gameObject.AddComponent<FishingInputHandler>();
            }

            FishingMinigameController controller = panel.GetComponent<FishingMinigameController>();
            if (controller == null)
            {
                controller = panel.gameObject.AddComponent<FishingMinigameController>();
            }

            controller.SetUiReferences(barImage, successImage, indicatorImage, messageText, attemptsText, targetFishText, hitsText);
            fishingLauncher.Configure(controller, fishingState, stateText, resultText);
            return controller;
        }

        [ContextMenu("Build Map Fishing Minigame UI")]
        private void BuildFromContextMenu()
        {
            BuildIfNeeded();
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
