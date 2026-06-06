using UnityEngine;
using UnityEngine.UI;

namespace SecretBoss
{
    public class SecretBossCombatUI : MonoBehaviour
    {
        [Header("Player Health UI")]
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Text playerHealthText;

        [Header("Boss Health UI")]
        [SerializeField] private Slider bossHealthSlider;
        [SerializeField] private SecretBossHealth bossHealth;
        [SerializeField] private Text bossHealthText;

        private void Start()
        {
            InitializeUI();
        }

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.HealthChanged += UpdatePlayerUI;
            }

            if (bossHealth != null)
            {
                bossHealth.HealthChanged += UpdateBossUI;
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.HealthChanged -= UpdatePlayerUI;
            }

            if (bossHealth != null)
            {
                bossHealth.HealthChanged -= UpdateBossUI;
            }
        }

        private void InitializeUI()
        {
            if (playerHealth != null)
            {
                UpdatePlayerUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            if (bossHealth != null)
            {
                UpdateBossUI(bossHealth.CurrentHealth, bossHealth.MaxHealth);
            }
        }

        private void UpdatePlayerUI(int current, int max)
        {
            if (playerHealthSlider != null)
            {
                playerHealthSlider.maxValue = max;
                playerHealthSlider.value = current;
            }

            if (playerHealthText != null)
            {
                playerHealthText.text = $"HP: {current}/{max}";
            }
        }

        private void UpdateBossUI(int current, int max)
        {
            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = max;
                bossHealthSlider.value = current;
            }

            if (bossHealthText != null)
            {
                bossHealthText.text = $"Boss: {current}/{max}";
            }
        }
    }
}