using UnityEngine;

namespace MantaMinigames.Fishing
{
    // Permite que una zona de trigger del mapa abra el FishingSpot sin duplicar logica de pesca.
    public sealed class FishingZoneInteractionProxy : MonoBehaviour, global::IInteractable
    {
        [SerializeField] private global::FishingSpot fishingSpot;

        public void Configure(global::FishingSpot targetFishingSpot)
        {
            if (targetFishingSpot != null)
            {
                fishingSpot = targetFishingSpot;
            }
        }

        public void Interact(global::PlayerInteractor interactor)
        {
            ResolveFishingSpot()?.Interact(interactor);
        }

        public string GetInteractionPrompt()
        {
            global::FishingSpot target = ResolveFishingSpot();
            return target != null ? target.GetInteractionPrompt() : "Presiona E para pescar";
        }

        private global::FishingSpot ResolveFishingSpot()
        {
            if (fishingSpot != null)
            {
                return fishingSpot;
            }

            GameObject muelleSpot = GameObject.Find("FishingSpot_Muelle");
            if (muelleSpot != null)
            {
                fishingSpot = muelleSpot.GetComponent<global::FishingSpot>();
            }

            return fishingSpot;
        }
    }
}
