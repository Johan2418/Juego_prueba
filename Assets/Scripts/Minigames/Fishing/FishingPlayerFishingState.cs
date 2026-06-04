using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Estado temporal para probar integracion antes de conectar inventario real.
    public sealed class FishingPlayerFishingState : MonoBehaviour
    {
        [SerializeField] private bool hasFish;

        public bool HasFish => hasFish;

        public void SetHasFish(bool value)
        {
            hasFish = value;
        }

        public void ClearFish()
        {
            SetHasFish(false);
        }
    }
}
