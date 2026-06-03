using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MantaMinigames.Fishing
{
    // Controla un intento de pesca aislado y emite un resultado final.
    [RequireComponent(typeof(FishingInputHandler))]
    public sealed class FishingMinigameController : MonoBehaviour
    {
        [SerializeField] private FishingInputHandler inputHandler;
        [SerializeField] private FishingRewardData rewardData;
        [SerializeField] private bool startOnEnable;
        [SerializeField] private bool logResultToConsole = true;
        [SerializeField, Min(1)] private int maxAttempts = 3;
        [SerializeField, Min(0.05f)] private float indicatorSpeed = 0.75f;
        [SerializeField, Range(0f, 1f)] private float initialIndicatorPosition;
        [SerializeField, Range(0f, 1f)] private float successWindowStart = 0.45f;
        [SerializeField, Range(0f, 1f)] private float successWindowEnd = 0.7f;
        [SerializeField] private Image barImage;
        [SerializeField] private Image successZoneImage;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI attemptsText;

        private const string SuccessMessage = "Conseguiste un pescado";
        private const string RetryMessage = "Intenta otra vez";

        [SerializeField] private bool hasFish;

        private float indicatorPosition;
        private int indicatorDirection = 1;

        public event Action<FishingResult> MinigameFinished;
        public event Action<FishingResult> AttemptResolved;

        public bool IsRunning { get; private set; }
        public bool HasFish => hasFish;
        public int RemainingAttempts { get; private set; }
        public FishingResult LastResult { get; private set; }
        public float NormalizedProgress => Mathf.Clamp01(indicatorPosition);

        private void Awake()
        {
            if (inputHandler == null)
            {
                inputHandler = GetComponent<FishingInputHandler>();
            }

            RefreshVisuals();
        }

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartMinigame();
            }
        }

        private void Update()
        {
            if (!IsRunning || inputHandler == null)
            {
                return;
            }

            MoveIndicator(Time.deltaTime);

            if (inputHandler.ConsumeCancelPressed())
            {
                Finish(FishingResultStatus.Failed);
                return;
            }

            if (inputHandler.ConsumeConfirmPressed())
            {
                TryCatchAtCurrentPosition();
            }
        }

        public void SetRewardData(FishingRewardData reward)
        {
            rewardData = reward;
        }

        public void StartMinigame()
        {
            indicatorPosition = initialIndicatorPosition;
            indicatorDirection = 1;
            RemainingAttempts = Mathf.Max(1, maxAttempts);
            hasFish = false;
            IsRunning = true;
            inputHandler?.ResetInput();
            SetMessage(string.Empty);
            RefreshVisuals();
        }

        public void CancelMinigame()
        {
            if (IsRunning)
            {
                Finish(FishingResultStatus.Failed);
            }
        }

        public bool IsProgressInSuccessWindow(float normalizedProgress)
        {
            return normalizedProgress >= successWindowStart && normalizedProgress <= successWindowEnd;
        }

        public FishingResult TryCatchAtCurrentPosition()
        {
            if (!IsRunning)
            {
                return LastResult;
            }

            if (IsProgressInSuccessWindow(NormalizedProgress))
            {
                hasFish = true;
                return Finish(FishingResultStatus.Success);
            }

            RemainingAttempts = Mathf.Max(0, RemainingAttempts - 1);

            if (RemainingAttempts <= 0)
            {
                return Finish(FishingResultStatus.Failed);
            }

            LastResult = CreateResult(FishingResultStatus.InProgress);
            SetMessage(LastResult.Message);
            RefreshVisuals();
            AttemptResolved?.Invoke(LastResult);
            return LastResult;
        }

        public FishingResult CreateResult(FishingResultStatus status)
        {
            FishingRewardData resultReward = status == FishingResultStatus.Success ? rewardData : null;
            string message = status == FishingResultStatus.Success ? SuccessMessage : RetryMessage;

            return new FishingResult(status, resultReward, message, NormalizedProgress, RemainingAttempts);
        }

        public void RefreshVisuals()
        {
            PositionSuccessZone();
            PositionIndicator();
            UpdateAttemptsText();
        }

        public void ConfigureForTest(int attempts, float successStart, float successEnd, float currentIndicatorPosition)
        {
            maxAttempts = Mathf.Max(1, attempts);
            RemainingAttempts = maxAttempts;
            successWindowStart = Mathf.Clamp01(successStart);
            successWindowEnd = Mathf.Clamp01(Mathf.Max(successWindowStart, successEnd));
            initialIndicatorPosition = Mathf.Clamp01(currentIndicatorPosition);
            indicatorPosition = initialIndicatorPosition;
        }

        public void SetUiReferencesForTest(Image bar, Image successZone, Image indicator)
        {
            SetUiReferences(bar, successZone, indicator, messageText, attemptsText);
        }

        public void SetUiReferences(
            Image bar,
            Image successZone,
            Image indicator,
            TextMeshProUGUI message,
            TextMeshProUGUI attempts)
        {
            barImage = bar;
            successZoneImage = successZone;
            indicatorImage = indicator;
            messageText = message;
            attemptsText = attempts;
            RefreshVisuals();
        }

        private FishingResult Finish(FishingResultStatus status)
        {
            IsRunning = false;
            LastResult = CreateResult(status);
            SetMessage(LastResult.Message);
            RefreshVisuals();

            if (logResultToConsole)
            {
                Debug.Log(LastResult.Message);
            }

            MinigameFinished?.Invoke(LastResult);
            return LastResult;
        }

        private void MoveIndicator(float deltaTime)
        {
            indicatorPosition += indicatorDirection * indicatorSpeed * deltaTime;

            if (indicatorPosition >= 1f)
            {
                indicatorPosition = 1f;
                indicatorDirection = -1;
            }
            else if (indicatorPosition <= 0f)
            {
                indicatorPosition = 0f;
                indicatorDirection = 1;
            }

            PositionIndicator();
        }

        private void PositionSuccessZone()
        {
            if (barImage == null || successZoneImage == null)
            {
                return;
            }

            RectTransform barRect = barImage.rectTransform;
            RectTransform successRect = successZoneImage.rectTransform;
            float barWidth = barRect.rect.width > 0f ? barRect.rect.width : barRect.sizeDelta.x;
            float windowWidth = (successWindowEnd - successWindowStart) * barWidth;
            float center = ((successWindowStart + successWindowEnd) * 0.5f) * barWidth;

            successRect.anchorMin = new Vector2(0f, 0.5f);
            successRect.anchorMax = new Vector2(0f, 0.5f);
            successRect.pivot = new Vector2(0.5f, 0.5f);
            successRect.sizeDelta = new Vector2(windowWidth, successRect.sizeDelta.y);
            successRect.anchoredPosition = new Vector2(center, successRect.anchoredPosition.y);
        }

        private void PositionIndicator()
        {
            if (barImage == null || indicatorImage == null)
            {
                return;
            }

            RectTransform barRect = barImage.rectTransform;
            RectTransform indicatorRect = indicatorImage.rectTransform;
            float barWidth = barRect.rect.width > 0f ? barRect.rect.width : barRect.sizeDelta.x;

            indicatorRect.anchorMin = new Vector2(0f, 0.5f);
            indicatorRect.anchorMax = new Vector2(0f, 0.5f);
            indicatorRect.pivot = new Vector2(0.5f, 0.5f);
            indicatorRect.anchoredPosition = new Vector2(indicatorPosition * barWidth, indicatorRect.anchoredPosition.y);
        }

        private void SetMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        private void UpdateAttemptsText()
        {
            if (attemptsText != null)
            {
                attemptsText.text = $"Intentos: {RemainingAttempts}";
            }
        }

        private void OnValidate()
        {
            maxAttempts = Mathf.Max(1, maxAttempts);
            indicatorSpeed = Mathf.Max(0.05f, indicatorSpeed);
            initialIndicatorPosition = Mathf.Clamp01(initialIndicatorPosition);
            successWindowStart = Mathf.Clamp01(successWindowStart);
            successWindowEnd = Mathf.Clamp01(Mathf.Max(successWindowStart, successWindowEnd));
            RefreshVisuals();
        }
    }
}
