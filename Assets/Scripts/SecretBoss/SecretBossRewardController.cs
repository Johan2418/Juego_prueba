using UnityEngine;

namespace SecretBoss
{
    public class SecretBossRewardController : MonoBehaviour
    {
        [SerializeField] private SecretBossHealth bossHealth;

        private void OnEnable()
        {
            if (bossHealth != null)
            {
                bossHealth.Defeated += HandleBossDefeated;
            }
        }

        private void OnDisable()
        {
            if (bossHealth != null)
            {
                bossHealth.Defeated -= HandleBossDefeated;
            }
        }

        private void HandleBossDefeated()
        {
            // Position the reward where the boss was
            transform.position = bossHealth.transform.position;
            gameObject.SetActive(true);
            Debug.Log("[SecretBossRewardController] Recompensa aparecida.");
        }
    }
}
