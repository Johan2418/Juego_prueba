using UnityEngine;

namespace MantaMinigames.RapidEat
{
    // Adaptador exclusivo para el puesto de ceviche del mapa.
    [DisallowMultipleComponent]
    [AddComponentMenu("Manta Minigames/Rapid Eat/Ceviche Interact Adapter")]
    public sealed class RapidEatCevicheInteractAdapter : MonoBehaviour, global::IInteractable
    {
        [SerializeField] private string prompt = "Presiona E para pedir ceviche";
        [SerializeField] private RapidEatCevicheIntegration integration;

        public void Interact(global::PlayerInteractor interactor)
        {
            ResolveIntegration();

            if (integration == null)
            {
                Debug.LogWarning("RapidEatCevicheInteractAdapter: falta RapidEatCevicheIntegration en el puesto de ceviche.");
                return;
            }

            integration.StartRapidEatFromCeviche();
        }

        public string GetInteractionPrompt()
        {
            return prompt;
        }

        private void Awake()
        {
            ResolveIntegration();
        }

        private void ResolveIntegration()
        {
            if (integration == null)
            {
                integration = GetComponent<RapidEatCevicheIntegration>();
            }
        }
    }
}
