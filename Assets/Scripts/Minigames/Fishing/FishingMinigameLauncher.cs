using System;
using TMPro;
using UnityEngine;
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

            if (minigameController != null)
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
                AssignMinigameController(UnityEngine.Object.FindFirstObjectByType<FishingMinigameController>());

                if (minigameController == null)
                {
                    FishingMapMinigameSceneBuilder builder = ResolveSceneBuilder();
                    if (builder != null)
                    {
                        AssignMinigameController(builder.BuildIfNeeded());
                    }
                }

                if (minigameController == null)
                {
                    AssignMinigameController(UnityEngine.Object.FindFirstObjectByType<FishingMinigameController>());
                }
            }
            finally
            {
                isAutoConfiguring = false;
            }

            return minigameController != null;
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

            fishingState = UnityEngine.Object.FindFirstObjectByType<FishingPlayerFishingState>();
            if (fishingState != null)
            {
                return fishingState;
            }

            fishingState = gameObject.AddComponent<FishingPlayerFishingState>();
            return fishingState;
        }

        private FishingMapMinigameSceneBuilder ResolveSceneBuilder()
        {
            FishingMapMinigameSceneBuilder builder = UnityEngine.Object.FindFirstObjectByType<FishingMapMinigameSceneBuilder>();
            Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

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

            builder.ConfigureSceneReferences(canvas, this, ResolveFishingState());
            return builder;
        }

        private static Canvas CreateRuntimeCanvas()
        {
            GameObject canvasObject = new GameObject("FishingRuntimeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
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

            if (isActiveAndEnabled && minigameController != null)
            {
                minigameController.OnFishingCompleted += HandleFishingCompleted;
            }
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
            return Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.RightArrow);
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
