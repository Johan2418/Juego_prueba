using System;
using TMPro;
using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Puente aislado entre una zona/boton de pesca y el minijuego.
    public sealed class FishingMinigameLauncher : MonoBehaviour
    {
        [SerializeField] private FishingMinigameController minigameController;
        [SerializeField] private FishingPlayerFishingState fishingState;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI resultText;

        public event Action<FishingResult> OnFishingCompleted;

        public bool HasMinigameController => minigameController != null;

        private void OnEnable()
        {
            if (minigameController != null)
            {
                minigameController.OnFishingCompleted += HandleFishingCompleted;
            }

            RefreshStateText();
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
            if (minigameController == null)
            {
                Debug.LogWarning("FishingMinigameLauncher necesita una referencia a FishingMinigameController.");
                return;
            }

            if (fishingState != null)
            {
                fishingState.ClearFish();
            }

            SetResultText("Pesca iniciada");
            RefreshStateText();
            minigameController.StartMinigame();
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
            if (minigameController != null)
            {
                minigameController.OnFishingCompleted -= HandleFishingCompleted;
            }

            minigameController = controller;
            fishingState = state;
            stateText = stateLabel;
            resultText = resultLabel;

            if (isActiveAndEnabled && minigameController != null)
            {
                minigameController.OnFishingCompleted += HandleFishingCompleted;
            }

            RefreshStateText();
        }

        private void HandleFishingCompleted(FishingResult result)
        {
            bool caughtFish = result == FishingResult.Success;

            if (fishingState != null)
            {
                fishingState.SetHasFish(caughtFish);
            }

            SetResultText($"Resultado: {result}");
            RefreshStateText();
            OnFishingCompleted?.Invoke(result);
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
