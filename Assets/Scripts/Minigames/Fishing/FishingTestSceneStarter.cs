using UnityEngine;
using UnityEngine.InputSystem;

namespace MantaMinigames.Fishing
{
    // Helper solo para la escena de prueba; no integra con sistemas globales.
    public sealed class FishingTestSceneStarter : MonoBehaviour
    {
        [SerializeField] private FishingMinigameController minigameController;
        [SerializeField] private Key restartKey = Key.R;

        private void Awake()
        {
            if (minigameController == null)
            {
                minigameController = GetComponent<FishingMinigameController>();
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[restartKey].wasPressedThisFrame)
            {
                StartFishingTest();
            }
        }

        public void StartFishingTest()
        {
            minigameController?.StartMinigame();
        }
    }
}
