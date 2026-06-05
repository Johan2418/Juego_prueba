using UnityEngine;
using System.Collections.Generic;

namespace BusMinigame
{
    public class BusMinigameManager : MonoBehaviour
    {
        public static BusMinigameManager Instance { get; private set; }

        [Header("Game Settings")]
        public float timeLimit = 60f;
        public int maxCrashes = 3;
        
        [Header("State")]
        public float currentTime;
        public int currentCrashes;
        public bool isGameActive;
        public int nextCheckpointIndex = 0;

        [Header("References")]
        public List<BusCheckpoint> checkpoints = new List<BusCheckpoint>();
        public BusUIController uiController;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            currentTime = timeLimit;
            currentCrashes = 0;
            nextCheckpointIndex = 0;
            isGameActive = true;
            
            if (uiController != null)
                uiController.UpdateUI();
        }

        void Update()
        {
            if (!isGameActive) return;

            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                GameOver("¡Se acabó el tiempo!");
            }

            if (uiController != null)
                uiController.UpdateUI();
        }

        public void OnCheckpointReached(BusCheckpoint checkpoint)
        {
            if (!isGameActive) return;

            int index = checkpoints.IndexOf(checkpoint);
            if (index == nextCheckpointIndex)
            {
                nextCheckpointIndex++;
                checkpoint.SetReached(true);
                Debug.Log($"Checkpoint {index + 1} alcanzado.");
                
                if (uiController != null)
                    uiController.ShowMessage($"¡Checkpoint {index + 1}!", 2f);
            }
        }

        public void OnObstacleHit()
        {
            if (!isGameActive) return;

            currentCrashes++;
            Debug.Log($"Choque {currentCrashes}/{maxCrashes}");
            
            if (uiController != null)
            {
                uiController.UpdateUI();
                uiController.ShowMessage("¡Choque!", 1f);
            }

            if (currentCrashes >= maxCrashes)
            {
                GameOver("Demasiados choques.");
            }
        }

        public void OnDestinationReached()
        {
            if (!isGameActive) return;

            if (nextCheckpointIndex >= checkpoints.Count)
            {
                Win();
            }
            else
            {
                if (uiController != null)
                    uiController.ShowMessage("Faltan checkpoints", 2f);
            }
        }

        void GameOver(string reason)
        {
            isGameActive = false;
            Debug.Log("Game Over: " + reason);
            if (uiController != null)
                uiController.ShowResult(false, reason);
        }

        void Win()
        {
            isGameActive = false;
            Debug.Log("¡Victoria!");
            if (uiController != null)
                uiController.ShowResult(true, "¡Has llegado al Malecón!");
        }
    }
}