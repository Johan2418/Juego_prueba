using TMPro;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace MantaMinigames.RapidEat
{
    // Sincroniza la UI basica del minijuego con el controlador RapidEat.
    public sealed class RapidEatUI : MonoBehaviour
    {
        [SerializeField] private RapidEatMinigameController minigameController;
        [SerializeField] private Text titleText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text resultText;
        [SerializeField] private TMP_Text titleTmpText;
        [SerializeField] private TMP_Text timeTmpText;
        [SerializeField] private TMP_Text resultTmpText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Image progressFillImage;

        private const string Title = "\u00a1Come r\u00e1pido!";
        private const string WinResult = "\u00a1Ganaste!";
        private const string LoseResult = "Perdiste";
        private const string EmptyResult = "";
        private const string TitleObjectName = "Text_Title";
        private const string SliderObjectName = "Slider_Progress";
        private const string TimeObjectName = "Text_Time";
        private const string ResultObjectName = "Text_Result";
        private const int TitleFontSize = 36;
        private const int TimeFontSize = 32;
        private const int ResultFontSize = 32;
        private const int InstructionsFontSize = 24;

        [SerializeField] private Text instructionsText;
        [SerializeField] private TMP_Text instructionsTmpText;
        private const string Instructions = "Presiona ESPACIO, clic o toca la pantalla";
        private const string InstructionsObjectName = "Text_Instructions";

        private RectTransform progressFillRect;
        private bool isSubscribed;
        private bool warnedMissingReferences;

        private void Awake()
        {
            AutoWireReferences();
            ConfigureProgressSlider();
            ConfigureProgressFillImage();
            ConfigureTextVisuals();
            SetInitialTexts();
            SetTextColors();
            WarnMissingReferencesIfNeeded();
        }

        private void OnEnable()
        {
            AutoWireReferences();
            Subscribe();
            RefreshAll();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void RefreshAll()
        {
            AutoWireReferences();

            if (minigameController == null)
            {
                WarnMissingReferencesIfNeeded();
                return;
            }

            SetInitialTexts();
            SetTextColors();
            ConfigureTextVisuals();
            UpdateProgress(minigameController.NormalizedProgress);
            UpdateTime(minigameController.TimeRemaining);
            SetResult(minigameController.HasFinished
                ? minigameController.WasSuccessful ? WinResult : LoseResult
                : EmptyResult);
            WarnMissingReferencesIfNeeded();
        }

        private void AutoWireReferences()
        {
            if (minigameController == null)
            {
                minigameController = GetComponent<RapidEatMinigameController>();
            }

            Transform sliderTransform = FindChildByName(SliderObjectName);
            if (progressSlider == null && sliderTransform != null)
            {
                progressSlider = sliderTransform.GetComponent<Slider>();
            }

            if (progressSlider == null)
            {
                progressSlider = GetComponentInChildren<Slider>(true);
            }

            if (progressFillImage == null)
            {
                Transform fillTransform = FindChildByName("Fill");
                if (fillTransform != null)
                {
                    progressFillImage = fillTransform.GetComponent<Image>();
                    progressFillRect = fillTransform as RectTransform;
                }
            }

            if (progressFillRect == null && progressFillImage != null)
            {
                progressFillRect = progressFillImage.rectTransform;
            }

            WireText(TitleObjectName, ref titleText, ref titleTmpText);
            WireText(TimeObjectName, ref timeText, ref timeTmpText);
            WireText(ResultObjectName, ref resultText, ref resultTmpText);
            WireText(InstructionsObjectName, ref instructionsText, ref instructionsTmpText);
        }

        private void ConfigureTextVisuals()
        {
            ConfigureTextComponent(titleText, titleTmpText, TitleFontSize);
            ConfigureTextComponent(timeText, timeTmpText, TimeFontSize);
            ConfigureTextComponent(resultText, resultTmpText, ResultFontSize);
            ConfigureTextComponent(instructionsText, instructionsTmpText, InstructionsFontSize);
        }

        private void ConfigureTextComponent(Text legacyText, TMP_Text tmpText, int fontSize)
        {
            if (legacyText != null)
            {
                legacyText.color = Color.white;
                legacyText.fontSize = fontSize;
                legacyText.alignment = TextAnchor.MiddleCenter;
                legacyText.horizontalOverflow = HorizontalWrapMode.Overflow;
                legacyText.verticalOverflow = VerticalWrapMode.Overflow;
            }

            if (tmpText != null)
            {
                tmpText.color = Color.white;
                tmpText.fontSize = fontSize;
                tmpText.alignment = TextAlignmentOptions.Center;
            }
        }

        private void Subscribe()
        {
            if (isSubscribed || minigameController == null)
            {
                return;
            }

            minigameController.OnMinigameStarted += HandleStarted;
            minigameController.OnMinigameWon += HandleWon;
            minigameController.OnMinigameLost += HandleLost;
            minigameController.OnProgressChanged += UpdateProgress;
            minigameController.OnTimeChanged += UpdateTime;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed || minigameController == null)
            {
                isSubscribed = false;
                return;
            }

            minigameController.OnMinigameStarted -= HandleStarted;
            minigameController.OnMinigameWon -= HandleWon;
            minigameController.OnMinigameLost -= HandleLost;
            minigameController.OnProgressChanged -= UpdateProgress;
            minigameController.OnTimeChanged -= UpdateTime;
            isSubscribed = false;
        }

        private void ConfigureProgressSlider()
        {
            if (progressSlider == null)
            {
                return;
            }

            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.wholeNumbers = false;
            progressSlider.interactable = false;
            progressSlider.value = 0f;
        }

        private void ConfigureProgressFillImage()
        {
            if (progressFillImage == null)
            {
                return;
            }

            progressFillImage.type = Image.Type.Filled;
            progressFillImage.fillMethod = Image.FillMethod.Horizontal;
            progressFillImage.fillOrigin = 0;
            progressFillImage.fillAmount = 0f;
        }

        private void HandleStarted()
        {
            SetResult(EmptyResult);
            UpdateProgress(0f);
            AutoWireReferences();
        }

        private void HandleWon()
        {
            UpdateProgress(1f);
            SetResult(WinResult);
        }

        private void HandleLost()
        {
            SetResult(LoseResult);
        }

        private void UpdateProgress(float normalizedProgress)
        {
            float safeProgress = Mathf.Clamp01(normalizedProgress);

            if (progressSlider != null)
            {
                progressSlider.value = safeProgress;
            }

            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = safeProgress;
            }

            if (progressFillRect != null)
            {
                progressFillRect.anchorMin = new Vector2(0f, progressFillRect.anchorMin.y);
                progressFillRect.anchorMax = new Vector2(safeProgress, progressFillRect.anchorMax.y);
                progressFillRect.offsetMin = new Vector2(0f, progressFillRect.offsetMin.y);
                progressFillRect.offsetMax = new Vector2(0f, progressFillRect.offsetMax.y);
            }
        }

        private void UpdateTime(float secondsRemaining)
        {
            float safeSeconds = Mathf.Max(0f, secondsRemaining);
            SetTime($"Tiempo: {safeSeconds.ToString("0.0", CultureInfo.InvariantCulture)}");
        }

        private void SetInitialTexts()
        {
            SetTitle();
            SetInstructions();
        }

        private void SetTitle()
        {
            SetText(titleText, titleTmpText, Title);
        }

        private void SetInstructions()
        {
            SetText(instructionsText, instructionsTmpText, Instructions);
        }

        private void SetTime(string message)
        {
            SetText(timeText, timeTmpText, message);
        }

        private void SetResult(string message)
        {
            SetText(resultText, resultTmpText, message);
        }

        private static void SetText(Text legacyText, TMP_Text tmpText, string message)
        {
            if (legacyText != null)
            {
                legacyText.text = message;
            }

            if (tmpText != null)
            {
                tmpText.text = message;
            }
        }

        private void SetTextColors()
        {
            foreach (Text text in GetComponentsInChildren<Text>(true))
            {
                text.color = Color.white;
            }

            foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = Color.white;
            }
        }

        private void WarnMissingReferencesIfNeeded()
        {
            if (warnedMissingReferences)
            {
                return;
            }

            bool hasMissingReference = false;

            if (minigameController == null)
            {
                Debug.LogWarning("RapidEatUI: falta referencia a RapidEatMinigameController.");
                hasMissingReference = true;
            }

            if (progressSlider == null)
            {
                Debug.LogWarning("RapidEatUI: no se encontro Slider en Slider_Progress. La barra usara el Image Fill si existe.");
                hasMissingReference = true;
            }

            if (progressFillImage == null && progressFillRect == null)
            {
                Debug.LogWarning("RapidEatUI: no se encontro Fill para la barra de progreso.");
                hasMissingReference = true;
            }

            if (titleText == null && titleTmpText == null)
            {
                Debug.LogWarning("RapidEatUI: falta Text o TMP_Text en Text_Title.");
                hasMissingReference = true;
            }

            if (timeText == null && timeTmpText == null)
            {
                Debug.LogWarning("RapidEatUI: falta Text o TMP_Text en Text_Time.");
                hasMissingReference = true;
            }

            if (resultText == null && resultTmpText == null)
            {
                Debug.LogWarning("RapidEatUI: falta Text o TMP_Text en Text_Result.");
                hasMissingReference = true;
            }

            if (instructionsText == null && instructionsTmpText == null)
            {
                Debug.LogWarning("RapidEatUI: falta Text o TMP_Text en Text_Instructions.");
                hasMissingReference = true;
            }

            warnedMissingReferences = hasMissingReference;
        }

        private void WireText(string objectName, ref Text legacyText, ref TMP_Text tmpText)
        {
            Transform textTransform = FindChildByName(objectName);
            if (textTransform == null)
            {
                return;
            }

            if (legacyText == null)
            {
                legacyText = textTransform.GetComponent<Text>();
            }

            if (tmpText == null)
            {
                tmpText = textTransform.GetComponent<TMP_Text>();
            }
        }

        private Transform FindChildByName(string childName)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }

        private void OnValidate()
        {
            AutoWireReferences();
            SetInitialTexts();
            SetTextColors();
            ConfigureTextVisuals();
        }
    }
}
