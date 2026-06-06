using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MantaMinigames.RapidEat
{
    // Controla un intento aislado del minijuego de comer rapido.
    public sealed class RapidEatMinigameController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float maxTime = 8f;
        [SerializeField, Min(0.1f)] private float progressPerPress = 8f;
        [SerializeField, Min(0f)] private float progressDecayPerSecond = 12f;
        [SerializeField, Min(1f)] private float requiredProgress = 100f;
        [SerializeField] private bool autoStart;

        private float currentProgress;
        private float timeRemaining;
        private bool hasFinished;

        public event Action OnMinigameStarted;
        public event Action OnMinigameWon;
        public event Action OnMinigameLost;
        public event Action<float> OnProgressChanged;
        public event Action<float> OnTimeChanged;

        public bool IsRunning { get; private set; }
        public bool HasFinished => hasFinished;
        public bool WasSuccessful { get; private set; }
        public float CurrentProgress => currentProgress;
        public float RequiredProgress => requiredProgress;
        public float NormalizedProgress => requiredProgress <= 0f ? 0f : Mathf.Clamp01(currentProgress / requiredProgress);
        public float TimeRemaining => timeRemaining;

        private void Awake()
        {
            ResetState();
        }

        private void Start()
        {
            if (autoStart)
            {
                StartMinigame();
            }
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            bool pressRegistered = WasEatInputPressedThisFrame();

            if (pressRegistered)
            {
                AddProgress(progressPerPress);
                Debug.Log($"RapidEat progress: {NormalizedProgress:0.00}");

                if (currentProgress >= requiredProgress)
                {
                    Finish(true);
                    return;
                }
            }
            else if (progressDecayPerSecond > 0f)
            {
                AddProgress(-progressDecayPerSecond * Time.deltaTime);
            }

            timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);
            OnTimeChanged?.Invoke(timeRemaining);

            if (timeRemaining <= 0f)
            {
                Finish(currentProgress >= requiredProgress);
            }
        }

        public void StartMinigame()
        {
            ResetState();
            IsRunning = true;
            OnMinigameStarted?.Invoke();
            NotifyStateChanged();
        }

        public void RestartMinigame()
        {
            StartMinigame();
        }

        private void ResetState()
        {
            currentProgress = 0f;
            timeRemaining = maxTime;
            IsRunning = false;
            hasFinished = false;
            WasSuccessful = false;
        }

        private void AddProgress(float amount)
        {
            float nextProgress = Mathf.Clamp(currentProgress + amount, 0f, requiredProgress);

            if (Mathf.Approximately(nextProgress, currentProgress))
            {
                return;
            }

            currentProgress = nextProgress;
            OnProgressChanged?.Invoke(NormalizedProgress);
        }

        private void Finish(bool success)
        {
            if (hasFinished)
            {
                return;
            }

            IsRunning = false;
            hasFinished = true;
            WasSuccessful = success;

            if (success)
            {
                currentProgress = requiredProgress;
                OnProgressChanged?.Invoke(NormalizedProgress);
                Debug.Log("RapidEat result: won");
                OnMinigameWon?.Invoke();
                return;
            }

            Debug.Log("RapidEat result: lost");
            OnMinigameLost?.Invoke();
        }

        private void NotifyStateChanged()
        {
            OnProgressChanged?.Invoke(NormalizedProgress);
            OnTimeChanged?.Invoke(timeRemaining);
        }

        private bool WasEatInputPressedThisFrame()
        {
            bool keyboardPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            bool touchPressed = WasTouchPressedThisFrame();
            bool inputPressed = keyboardPressed || mousePressed || touchPressed;

            if (inputPressed)
            {
                Debug.Log("RapidEat input detected");
            }

            return inputPressed;
        }

        private static bool WasTouchPressedThisFrame()
        {
            if (Touchscreen.current == null)
            {
                return false;
            }

            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                return true;
            }

            foreach (TouchControl touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            maxTime = Mathf.Max(0.1f, maxTime);
            progressPerPress = Mathf.Max(0.1f, progressPerPress);
            progressDecayPerSecond = Mathf.Max(0f, progressDecayPerSecond);
            requiredProgress = Mathf.Max(1f, requiredProgress);
        }
    }
}
