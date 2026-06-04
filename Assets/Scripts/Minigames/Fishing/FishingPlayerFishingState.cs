using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Estado temporal para probar integracion antes de conectar inventario real.
    public sealed class FishingPlayerFishingState : MonoBehaviour
    {
        [SerializeField] private bool hasFish;
        [SerializeField] private global::FishData caughtFish;

        public bool HasFish => hasFish;
        public global::FishData CaughtFish => caughtFish;

        public void SetHasFish(bool value)
        {
            hasFish = value;
            if (!hasFish)
            {
                caughtFish = null;
            }
        }

        public void SetCaughtFish(global::FishData fish)
        {
            caughtFish = fish;
            hasFish = true;
        }

        public void ClearFish()
        {
            SetHasFish(false);
        }
    }
}
