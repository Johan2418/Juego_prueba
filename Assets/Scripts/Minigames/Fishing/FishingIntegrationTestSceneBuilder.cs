using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MantaMinigames.Fishing
{
    // Arma una escena de integracion aislada para validar launcher + estado temporal.
    [ExecuteAlways]
    public sealed class FishingIntegrationTestSceneBuilder : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private const string IntegrationPanelName = "FishingIntegrationPanel";
        private const string StateObjectName = "FishingPlayerFishingState";
        private const string LauncherName = "FishingZoneDummy";
        private const string MinigamePanelName = "FishingMinigameUI";
        private const string BarName = "PrecisionBar";
        private const string SuccessName = "SuccessZone_Green";
        private const string IndicatorName = "MovingIndicator";
        private const string AttemptsName = "Text_Attempts";
        private const string MessageName = "Text_Message";
        private const string StateTextName = "Text_HasFish";
        private const string ResultTextName = "Text_Result";
        private const string StartButtonName = "Button_StartFishingFromZone";
        private const string ClearButtonName = "Button_ClearFish";

        private void Awake()
        {
            BuildIfNeeded();
        }

        private void OnEnable()
        {
            BuildIfNeeded();
        }

        [ContextMenu("Build Fishing Integration Test")]
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

            GameObject stateObject = GetOrCreateGameObject(StateObjectName);
            FishingPlayerFishingState fishingState = stateObject.GetComponent<FishingPlayerFishingState>();
            if (fishingState == null)
            {
                fishingState = stateObject.AddComponent<FishingPlayerFishingState>();
            }

            RectTransform integrationPanel = GetOrCreateRect(canvas.transform, IntegrationPanelName, new Vector2(360f, 220f));
            integrationPanel.anchorMin = new Vector2(0f, 1f);
            integrationPanel.anchorMax = new Vector2(0f, 1f);
            integrationPanel.pivot = new Vector2(0f, 1f);
            integrationPanel.anchoredPosition = new Vector2(30f, -30f);
            EnsureImage(integrationPanel.gameObject, new Color(0.08f, 0.1f, 0.12f, 0.94f));

            TextMeshProUGUI stateText = GetOrCreateText(integrationPanel, StateTextName, "HasFish: False", 22f, new Vector2(0f, 62f), new Vector2(300f, 34f));
            TextMeshProUGUI resultText = GetOrCreateText(integrationPanel, ResultTextName, "Resultado: None", 19f, new Vector2(0f, 18f), new Vector2(300f, 34f));
            Button startButton = GetOrCreateButton(integrationPanel, StartButtonName, "Iniciar pesca", new Vector2(0f, -42f), new Vector2(220f, 40f));
            Button clearButton = GetOrCreateButton(integrationPanel, ClearButtonName, "Limpiar pescado", new Vector2(0f, -94f), new Vector2(220f, 36f));

            RectTransform minigamePanel = GetOrCreateRect(canvas.transform, MinigamePanelName, new Vector2(520f, 260f));
            minigamePanel.anchorMin = new Vector2(0.5f, 0.5f);
            minigamePanel.anchorMax = new Vector2(0.5f, 0.5f);
            minigamePanel.pivot = new Vector2(0.5f, 0.5f);
            minigamePanel.anchoredPosition = new Vector2(120f, 0f);
            EnsureImage(minigamePanel.gameObject, new Color(0.07f, 0.09f, 0.11f, 0.92f));

            RectTransform bar = GetOrCreateRect(minigamePanel, BarName, new Vector2(420f, 32f));
            bar.anchoredPosition = new Vector2(0f, 32f);
            Image barImage = EnsureImage(bar.gameObject, new Color(0.18f, 0.2f, 0.24f, 1f));

            RectTransform successZone = GetOrCreateRect(bar, SuccessName, new Vector2(90f, 32f));
            Image successImage = EnsureImage(successZone.gameObject, new Color(0.16f, 0.72f, 0.32f, 1f));

            RectTransform indicator = GetOrCreateRect(bar, IndicatorName, new Vector2(12f, 52f));
            Image indicatorImage = EnsureImage(indicator.gameObject, new Color(1f, 0.92f, 0.32f, 1f));

            TextMeshProUGUI attemptsText = GetOrCreateText(minigamePanel, AttemptsName, "Intentos: 3", 24f, new Vector2(0f, 94f), new Vector2(420f, 36f));
            TextMeshProUGUI messageText = GetOrCreateText(minigamePanel, MessageName, "Pulsa el boton de la zona dummy para iniciar.", 21f, new Vector2(0f, -34f), new Vector2(460f, 48f));

            FishingMinigameController controller = minigamePanel.GetComponent<FishingMinigameController>();
            if (controller == null)
            {
                controller = minigamePanel.gameObject.AddComponent<FishingMinigameController>();
            }

            controller.SetUiReferences(barImage, successImage, indicatorImage, messageText, attemptsText);

            GameObject launcherObject = GetOrCreateGameObject(LauncherName);
            FishingMinigameLauncher launcher = launcherObject.GetComponent<FishingMinigameLauncher>();
            if (launcher == null)
            {
                launcher = launcherObject.AddComponent<FishingMinigameLauncher>();
            }

            launcher.Configure(controller, fishingState, stateText, resultText);

            if (Application.isPlaying)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(launcher.StartFishing);
                clearButton.onClick.RemoveAllListeners();
                clearButton.onClick.AddListener(launcher.ClearFish);
            }
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

        private static GameObject GetOrCreateGameObject(string objectName)
        {
            GameObject existing = FindRootObject(objectName);
            return existing != null ? existing : new GameObject(objectName);
        }

        private static GameObject FindRootObject(string objectName)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = activeScene.GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].name == objectName)
                {
                    return rootObjects[i];
                }
            }

            return null;
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
            TextMeshProUGUI label = GetOrCreateText(buttonRect, "Label", labelText, 20f, Vector2.zero, rectSize);
            label.raycastTarget = false;
            return button;
        }
    }
}
