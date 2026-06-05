using UnityEngine;

namespace BusMinigame
{
    // This script can be used for extra result logic if needed, 
    // but the logic is mostly in Manager and UI.
    // Keeping it simple as requested.
    public class BusMinigameResult : MonoBehaviour
    {
        public void OnClickRestart()
        {
            BusMinigameManager.Instance.uiController.RestartGame();
        }
    }
}