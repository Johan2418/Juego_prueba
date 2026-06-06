using UnityEngine;
using UnityEngine.InputSystem;

namespace MantaMinigames.RapidEat
{
    // Helper solo para RapidEatTest; no integra con sistemas globales.
    [DisallowMultipleComponent]
    [AddComponentMenu("Manta Minigames/Rapid Eat/Test Starter")]
    public sealed class RapidEatTestStarter : MonoBehaviour
    {
        [SerializeField] private RapidEatMinigameController minigameController;

        private void Awake()
        {
            ResolveController();
        }

        private void OnEnable()
        {
            ResolveController();

            if (minigameController == null)
            {
                Debug.LogWarning("RapidEatTestStarter: falta RapidEatMinigameController en este GameObject.");
                return;
            }

            minigameController.OnMinigameWon += LogWin;
            minigameController.OnMinigameLost += LogLoss;
        }

        private void OnDisable()
        {
            if (minigameController == null)
            {
                return;
            }

            minigameController.OnMinigameWon -= LogWin;
            minigameController.OnMinigameLost -= LogLoss;
        }

        private void Start()
        {
            RestartTest();
        }

        private void Update()
        {
            if (WasRestartPressedThisFrame())
            {
                RestartTest();
            }
        }

        public void RestartTest()
        {
            ResolveController();

            if (minigameController == null)
            {
                Debug.LogWarning("RapidEatTestStarter: no se pudo reiniciar porque falta RapidEatMinigameController.");
                return;
            }

            minigameController.RestartMinigame();
        }

        private static void LogWin()
        {
            Debug.Log("RapidEat test result: won");
        }

        private static void LogLoss()
        {
            Debug.Log("RapidEat test result: lost");
        }

        private bool WasRestartPressedThisFrame()
        {
            return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
        }

        private void ResolveController()
        {
            if (minigameController == null)
            {
                minigameController = GetComponent<RapidEatMinigameController>();
            }
        }
    }
}
