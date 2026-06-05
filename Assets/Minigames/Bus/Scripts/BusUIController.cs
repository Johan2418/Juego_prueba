using UnityEngine;
using TMPro;

namespace BusMinigame
{
    public class BusUIController : MonoBehaviour
    {
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI crashText;
        public TextMeshProUGUI messageText;
        public GameObject resultPanel;
        public TextMeshProUGUI resultTitle;
        public TextMeshProUGUI resultMessage;

        private float messageTimer;

        void Start()
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            if (messageText != null) messageText.text = "Lleva el bus desde el mercado hasta el malecón.";
            messageTimer = 3f;
        }

        void Update()
        {
            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0 && messageText != null)
                {
                    messageText.text = "";
                }
            }
        }

        public void UpdateUI()
        {
            var mgr = BusMinigameManager.Instance;
            if (timeText != null)
                timeText.text = $"Tiempo: {Mathf.CeilToInt(mgr.currentTime)}s";
            
            if (crashText != null)
                crashText.text = $"Choques: {mgr.currentCrashes}/{mgr.maxCrashes}";
        }

        public void ShowMessage(string msg, float duration)
        {
            if (messageText != null)
            {
                messageText.text = msg;
                messageTimer = duration;
            }
        }

        public void ShowResult(bool win, string msg)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
                resultTitle.text = win ? "¡VICTORIA!" : "DERROTA";
                resultMessage.text = msg;
            }
        }

        public void RestartGame()
        {
            // Simple reload or reset
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}