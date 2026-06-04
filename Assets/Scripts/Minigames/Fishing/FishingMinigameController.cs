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
        [SerializeField] private TextMeshProUGUI targetFishText;
        [SerializeField] private TextMeshProUGUI hitsText;

        private const string SuccessMessage = "Conseguiste un pescado";
        private const string RetryMessage = "Intenta otra vez";
        private const string FailedMessage = "Se acabaron los intentos";
        private const string CancelledMessage = "Pesca cancelada";
        private const string GoodPullMessage = "Buen tiron";

        [SerializeField] private bool hasFish;
        [SerializeField] private FishingResult fishingResult = FishingResult.None;
        [SerializeField] private global::FishData targetFish;

        private float indicatorPosition;
        private int indicatorDirection = 1;
        private int currentSuccessHits;
        private int requiredSuccessHits = 1;
        private float defaultIndicatorSpeed;
        private float defaultSuccessWindowStart;
        private float defaultSuccessWindowEnd;

        public event Action<FishingResult> OnFishingCompleted;

        public bool IsRunning { get; private set; }
        public bool HasFish => hasFish;
        public int RemainingAttempts { get; private set; }
        public FishingResult Result => fishingResult;
        public float NormalizedProgress => Mathf.Clamp01(indicatorPosition);
        public global::FishData TargetFish => targetFish;
        public int CurrentSuccessHits => currentSuccessHits;
        public int RequiredSuccessHits => requiredSuccessHits;

        private void Awake()
        {
            CacheDefaultDifficulty();

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
                CancelMinigame();
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

        public void SetTargetFish(global::FishData fish)
        {
            targetFish = fish;
            ApplyDifficultyForTargetFish();
            RefreshVisuals();
        }

        public void StartMinigame()
        {
            ApplyDifficultyForTargetFish();
            indicatorPosition = initialIndicatorPosition;
            indicatorDirection = 1;
            RemainingAttempts = Mathf.Max(1, maxAttempts);
            currentSuccessHits = 0;
            hasFish = false;
            fishingResult = FishingResult.None;
            IsRunning = true;
            inputHandler?.ResetInput();
            SetMessage(string.Empty);
            RefreshVisuals();
        }

        public void CancelMinigame()
        {
            if (IsRunning)
            {
                Finish(FishingResult.Cancelled);
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
                return Result;
            }

            if (IsProgressInSuccessWindow(NormalizedProgress))
            {
                currentSuccessHits = Mathf.Min(requiredSuccessHits, currentSuccessHits + 1);

                if (currentSuccessHits >= requiredSuccessHits)
                {
                    hasFish = true;
                    return Finish(FishingResult.Success);
                }

                fishingResult = FishingResult.None;
                SetMessage(GoodPullMessage);
                RefreshVisuals();
                return Result;
            }

            RemainingAttempts = Mathf.Max(0, RemainingAttempts - 1);

            if (RemainingAttempts <= 0)
            {
                return Finish(FishingResult.Failed);
            }

            fishingResult = FishingResult.None;
            SetMessage(RetryMessage);
            RefreshVisuals();
            return Result;
        }

        public void RefreshVisuals()
        {
            PositionSuccessZone();
            PositionIndicator();
            UpdateAttemptsText();
            UpdateTargetFishText();
            UpdateHitsText();
        }

        public void ConfigureForTest(int attempts, float successStart, float successEnd, float currentIndicatorPosition)
        {
            maxAttempts = Mathf.Max(1, attempts);
            RemainingAttempts = maxAttempts;
            successWindowStart = Mathf.Clamp01(successStart);
            successWindowEnd = Mathf.Clamp01(Mathf.Max(successWindowStart, successEnd));
            initialIndicatorPosition = Mathf.Clamp01(currentIndicatorPosition);
            indicatorPosition = initialIndicatorPosition;
            requiredSuccessHits = 1;
            currentSuccessHits = 0;
            CacheDefaultDifficulty();
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
            SetUiReferences(bar, successZone, indicator, message, attempts, targetFishText, hitsText);
        }

        public void SetUiReferences(
            Image bar,
            Image successZone,
            Image indicator,
            TextMeshProUGUI message,
            TextMeshProUGUI attempts,
            TextMeshProUGUI target,
            TextMeshProUGUI hits)
        {
            barImage = bar;
            successZoneImage = successZone;
            indicatorImage = indicator;
            messageText = message;
            attemptsText = attempts;
            targetFishText = target;
            hitsText = hits;
            RefreshVisuals();
        }

        private FishingResult Finish(FishingResult result)
        {
            IsRunning = false;
            fishingResult = result;
            SetMessage(GetMessageForResult(result));
            RefreshVisuals();

            if (logResultToConsole)
            {
                Debug.Log(GetMessageForResult(result));
            }

            OnFishingCompleted?.Invoke(fishingResult);
            return fishingResult;
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

        private void UpdateTargetFishText()
        {
            if (targetFishText == null)
            {
                return;
            }

            targetFishText.text = targetFish != null ? $"Pez: {targetFish.DisplayName}" : "Pez: Pescado";
        }

        private void UpdateHitsText()
        {
            if (hitsText != null)
            {
                hitsText.text = $"Aciertos: {currentSuccessHits}/{requiredSuccessHits}";
            }
        }

        private void ApplyDifficultyForTargetFish()
        {
            if (targetFish == null)
            {
                indicatorSpeed = defaultIndicatorSpeed;
                successWindowStart = defaultSuccessWindowStart;
                successWindowEnd = defaultSuccessWindowEnd;
                requiredSuccessHits = 1;
                return;
            }

            switch (targetFish.Rarity)
            {
                case global::FishRarity.Rare:
                    indicatorSpeed = 1.25f;
                    successWindowStart = 0.46f;
                    successWindowEnd = 0.61f;
                    requiredSuccessHits = 3;
                    break;
                case global::FishRarity.Uncommon:
                    indicatorSpeed = 0.9f;
                    successWindowStart = 0.4f;
                    successWindowEnd = 0.68f;
                    requiredSuccessHits = 2;
                    break;
                default:
                    indicatorSpeed = 0.65f;
                    successWindowStart = 0.32f;
                    successWindowEnd = 0.76f;
                    requiredSuccessHits = 1;
                    break;
            }
        }

        private void CacheDefaultDifficulty()
        {
            defaultIndicatorSpeed = indicatorSpeed;
            defaultSuccessWindowStart = successWindowStart;
            defaultSuccessWindowEnd = successWindowEnd;
        }

        private string GetMessageForResult(FishingResult result)
        {
            switch (result)
            {
                case FishingResult.Success:
                    return targetFish != null ? $"Conseguiste: {targetFish.DisplayName}" : SuccessMessage;
                case FishingResult.Failed:
                    return FailedMessage;
                case FishingResult.Cancelled:
                    return CancelledMessage;
                default:
                    return RetryMessage;
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
