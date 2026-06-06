using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace MantaMinigames.Fishing
{
    // Puente aislado entre una zona/boton de pesca y el minijuego.
    public sealed class FishingMinigameLauncher : MonoBehaviour
    {
        [SerializeField] private FishingMinigameController minigameController;
        [SerializeField] private FishingPlayerFishingState fishingState;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private GameObject minigameRoot;
        [SerializeField, Min(0f)] private float minimumResultVisibleSeconds = 0.75f;
        [SerializeField] private bool hideOwnMarkerRenderer = true;

        private bool isAutoConfiguring;
        private bool waitingToHideOnPlayerMove;
        private float hideAllowedTime;

        private const string RuntimeCanvasName = "GameplayRuntimeCanvas";
        private const string RuntimeEventSystemName = "FishingRuntimeEventSystem";

        public event Action<FishingResult> OnFishingCompleted;

        public bool HasMinigameController
        {
            get
            {
                EnsureConfigured();
                return minigameController != null;
            }
        }

        private void OnEnable()
        {
            HideOwnMarkerRendererIfNeeded();

            if (minigameController != null)
            {
                minigameController.OnFishingCompleted += HandleFishingCompleted;
            }

            RefreshStateText();
            HideMinigameUI();
        }

        private void OnDisable()
        {
            if (minigameController != null)
            {
                minigameController.OnFishingCompleted -= HandleFishingCompleted;
            }
        }

        public void StartFishing()
        {
            StartFishing(null);
        }

        public void StartFishing(global::FishData selectedFish)
        {
            EnsureConfigured();
            RebuildRuntimeUiIfNeeded();

            if (minigameController == null)
            {
                Debug.LogWarning("FishingMinigameLauncher necesita una referencia a FishingMinigameController.");
                return;
            }

            waitingToHideOnPlayerMove = false;
            minigameController.SetTargetFish(selectedFish);
            ShowMinigameUI();

            if (fishingState != null)
            {
                fishingState.ClearFish();
            }

            SetResultText("Pesca iniciada");
            RefreshStateText();
            minigameController.StartMinigame();
        }

        private void Update()
        {
            if (!waitingToHideOnPlayerMove || Time.time < hideAllowedTime)
            {
                return;
            }

            if (HasPlayerMovementInput())
            {
                waitingToHideOnPlayerMove = false;
                HideMinigameUI();
            }
        }

        public void ClearFish()
        {
            fishingState?.ClearFish();
            SetResultText("Estado reiniciado");
            RefreshStateText();
        }

        public void SetVisualReferences(TextMeshProUGUI stateLabel, TextMeshProUGUI resultLabel)
        {
            stateText = stateLabel;
            resultText = resultLabel;
            RefreshStateText();
        }

        public void Configure(FishingMinigameController controller, FishingPlayerFishingState state, TextMeshProUGUI stateLabel, TextMeshProUGUI resultLabel)
        {
            AssignMinigameController(controller);
            fishingState = state;
            stateText = stateLabel;
            resultText = resultLabel;
            if (minigameRoot == null && minigameController != null)
            {
                minigameRoot = minigameController.gameObject;
            }

            RefreshStateText();
            if (minigameController == null || !minigameController.IsRunning)
            {
                HideMinigameUI();
            }
        }

        public bool EnsureConfigured()
        {
            ResolveFishingState();

            if (HasUsableController())
            {
                return true;
            }

            if (isAutoConfiguring)
            {
                return false;
            }

            isAutoConfiguring = true;
            try
            {
                AssignMinigameController(UnityEngine.Object.FindFirstObjectByType<FishingMinigameController>(FindObjectsInactive.Include));

                if (!HasUsableController())
                {
                    FishingMapMinigameSceneBuilder builder = ResolveSceneBuilder();
                    if (builder != null)
                    {
                        AssignMinigameController(builder.BuildIfNeeded());
                    }
                }

                if (!HasUsableController())
                {
                    AssignMinigameController(UnityEngine.Object.FindFirstObjectByType<FishingMinigameController>(FindObjectsInactive.Include));
                }
            }
            finally
            {
                isAutoConfiguring = false;
            }

            return HasUsableController();
        }

        private FishingPlayerFishingState ResolveFishingState()
        {
            if (fishingState != null)
            {
                return fishingState;
            }

            fishingState = GetComponent<FishingPlayerFishingState>();
            if (fishingState != null)
            {
                return fishingState;
            }

            fishingState = UnityEngine.Object.FindFirstObjectByType<FishingPlayerFishingState>(FindObjectsInactive.Include);
            if (fishingState != null)
            {
                return fishingState;
            }

            fishingState = gameObject.AddComponent<FishingPlayerFishingState>();
            return fishingState;
        }

        private FishingMapMinigameSceneBuilder ResolveSceneBuilder()
        {
            FishingMapMinigameSceneBuilder builder = UnityEngine.Object.FindFirstObjectByType<FishingMapMinigameSceneBuilder>(FindObjectsInactive.Include);
            Canvas canvas = builder != null ? builder.GetComponent<Canvas>() : null;

            if (canvas == null)
            {
                canvas = FindGameplayCanvas();
            }

            if (builder == null)
            {
                if (canvas == null)
                {
                    canvas = CreateRuntimeCanvas();
                }

                builder = canvas.GetComponent<FishingMapMinigameSceneBuilder>();
                if (builder == null)
                {
                    builder = canvas.gameObject.AddComponent<FishingMapMinigameSceneBuilder>();
                }
            }

            EnsureEventSystem();
            builder.ConfigureSceneReferences(canvas, this, ResolveFishingState());
            return builder;
        }

        private void RebuildRuntimeUiIfNeeded()
        {
            FishingMapMinigameSceneBuilder builder = ResolveSceneBuilder();
            if (builder == null)
            {
                return;
            }

            FishingMinigameController rebuiltController = builder.BuildIfNeeded();
            if (rebuiltController != null)
            {
                AssignMinigameController(rebuiltController);
            }
        }

        private static Canvas CreateRuntimeCanvas()
        {
            GameObject canvasObject = new GameObject(RuntimeCanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static Canvas FindGameplayCanvas()
        {
            GameObject namedCanvas = GameObject.Find(RuntimeCanvasName);
            Canvas canvas = namedCanvas != null ? namedCanvas.GetComponent<Canvas>() : null;
            if (canvas != null)
            {
                return canvas;
            }

            Canvas[] canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < canvases.Length; i++)
            {
                Canvas candidate = canvases[i];
                if (candidate != null &&
                    (candidate.renderMode == RenderMode.ScreenSpaceOverlay || candidate.renderMode == RenderMode.ScreenSpaceCamera))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include) != null)
            {
                return;
            }

            _ = new GameObject(RuntimeEventSystemName, typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private void AssignMinigameController(FishingMinigameController controller)
        {
            if (minigameController == controller)
            {
                return;
            }

            if (minigameController != null)
            {
                minigameController.OnFishingCompleted -= HandleFishingCompleted;
            }

            minigameController = controller;
            if (minigameController != null)
            {
                minigameRoot = minigameController.gameObject;
            }

            if (isActiveAndEnabled && minigameController != null)
            {
                minigameController.OnFishingCompleted += HandleFishingCompleted;
            }
        }

        private bool HasUsableController()
        {
            if (minigameController == null)
            {
                return false;
            }

            if (minigameRoot == null)
            {
                minigameRoot = minigameController.gameObject;
            }

            return minigameRoot != null;
        }

        private void HandleFishingCompleted(FishingResult result)
        {
            bool caughtFish = result == FishingResult.Success;

            if (fishingState != null)
            {
                if (caughtFish)
                {
                    fishingState.SetCaughtFish(minigameController != null ? minigameController.TargetFish : null);
                }
                else
                {
                    fishingState.ClearFish();
                }
            }

            SetResultText($"Resultado: {result}");
            RefreshStateText();
            waitingToHideOnPlayerMove = false;
            HideMinigamePanelOnly();
            if (result == FishingResult.Cancelled)
            {
                minigameController?.HideWorldVisuals();
            }

            OnFishingCompleted?.Invoke(result);
        }

        private void ShowMinigameUI()
        {
            GameObject root = ResolveMinigameRoot();
            if (root != null)
            {
                root.SetActive(true);
            }
        }

        private void HideMinigameUI()
        {
            HideMinigamePanelOnly();
            minigameController?.HideWorldVisuals();
        }

        private void HideMinigamePanelOnly()
        {
            GameObject root = ResolveMinigameRoot();
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private GameObject ResolveMinigameRoot()
        {
            if (minigameRoot != null)
            {
                return minigameRoot;
            }

            if (minigameController != null)
            {
                minigameRoot = minigameController.gameObject;
            }

            return minigameRoot;
        }

        private static bool HasPlayerMovementInput()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null &&
                (keyboard.wKey.isPressed ||
                keyboard.aKey.isPressed ||
                keyboard.sKey.isPressed ||
                keyboard.dKey.isPressed ||
                keyboard.upArrowKey.isPressed ||
                keyboard.downArrowKey.isPressed ||
                keyboard.leftArrowKey.isPressed ||
                keyboard.rightArrowKey.isPressed);
        }

        private void HideOwnMarkerRendererIfNeeded()
        {
            if (!hideOwnMarkerRenderer)
            {
                return;
            }

            SpriteRenderer markerRenderer = GetComponent<SpriteRenderer>();
            if (markerRenderer != null)
            {
                markerRenderer.enabled = false;
            }
        }

        private void RefreshStateText()
        {
            if (stateText == null)
            {
                return;
            }

            bool hasFish = fishingState != null && fishingState.HasFish;
            stateText.text = $"HasFish: {hasFish}";
            stateText.color = hasFish ? new Color(0.2f, 0.9f, 0.35f, 1f) : new Color(1f, 0.82f, 0.2f, 1f);
        }

        private void SetResultText(string message)
        {
            if (resultText != null)
            {
                resultText.text = message;
            }
        }
    }
}
