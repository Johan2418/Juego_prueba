using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Datos opcionales de recompensa; no escribe en inventario global.
    [CreateAssetMenu(menuName = "Manta/Minigames/Fishing/Reward Data", fileName = "FishingRewardData")]
    public sealed class FishingRewardData : ScriptableObject
    {
        [SerializeField] private string rewardId = "fish";
        [SerializeField] private string displayName = "Fish";
        [SerializeField] private Sprite icon;
        [SerializeField] private int amount = 1;
        [SerializeField] private string successMessage = "Pez capturado.";
        [SerializeField] private string failureMessage = "El pez escapo.";

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int Amount => Mathf.Max(1, amount);
        public string SuccessMessage => successMessage;
        public string FailureMessage => failureMessage;

        private void OnValidate()
        {
            amount = Mathf.Max(1, amount);
        }
    }
}
