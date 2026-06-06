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
        [Header("Interaction Zone")]
        [SerializeField] private bool ensureFrontInteractionZone = true;
        [SerializeField] private string interactionZoneName = "RapidEatInteractionZone";
        [SerializeField] private Vector2 zoneLocalOffset = new Vector2(0f, -0.9f);
        [SerializeField] private Vector2 zoneSize = new Vector2(2.4f, 1.2f);

        private GameObject runtimeInteractionZone;

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
            EnsureInteractionZone();
        }

        private void OnEnable()
        {
            ResolveIntegration();
            EnsureInteractionZone();
        }

        private void ResolveIntegration()
        {
            if (integration == null)
            {
                integration = GetComponent<RapidEatCevicheIntegration>();
            }
        }

        private void EnsureInteractionZone()
        {
            if (!ensureFrontInteractionZone)
            {
                return;
            }

            Transform zoneTransform = transform.Find(interactionZoneName);
            if (zoneTransform == null)
            {
                GameObject zone = new GameObject(interactionZoneName);
                zone.transform.SetParent(transform, false);
                zoneTransform = zone.transform;
            }

            runtimeInteractionZone = zoneTransform.gameObject;
            zoneTransform.localPosition = zoneLocalOffset;
            zoneTransform.localRotation = Quaternion.identity;
            zoneTransform.localScale = Vector3.one;

            BoxCollider2D zoneCollider = runtimeInteractionZone.GetComponent<BoxCollider2D>();
            if (zoneCollider == null)
            {
                zoneCollider = runtimeInteractionZone.AddComponent<BoxCollider2D>();
            }

            zoneCollider.enabled = true;
            zoneCollider.isTrigger = true;
            zoneCollider.offset = Vector2.zero;
            zoneCollider.size = new Vector2(
                Mathf.Max(0.25f, zoneSize.x),
                Mathf.Max(0.25f, zoneSize.y));

            RapidEatCevicheZoneInteractable zoneInteractable =
                runtimeInteractionZone.GetComponent<RapidEatCevicheZoneInteractable>();
            if (zoneInteractable == null)
            {
                zoneInteractable = runtimeInteractionZone.AddComponent<RapidEatCevicheZoneInteractable>();
            }

            zoneInteractable.Configure(this);
        }

        private void OnValidate()
        {
            zoneSize.x = Mathf.Max(0.25f, zoneSize.x);
            zoneSize.y = Mathf.Max(0.25f, zoneSize.y);
        }
    }
}
