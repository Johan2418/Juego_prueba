using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    [SerializeField] private string message = "Zona descubierta";
    [SerializeField] private bool once = true;
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated && once) return;
        
        if (other.CompareTag("Player"))
        {
            var interactor = other.GetComponentInChildren<PlayerInteractor>();
            if (interactor != null)
            {
                // Trigger_SecretZone usa el flujo de demo; otros triggers conservan su mensaje simple.
                if (DemoQuestRouteManager.Instance != null &&
                    DemoQuestRouteManager.Instance.CanHandleInteraction(gameObject.name))
                {
                    DemoQuestRouteManager.Instance.TryHandleInteraction(gameObject.name, interactor);
                    activated = DemoQuestRouteManager.Instance.DemoFinished;
                    return;
                }

                interactor.ShowNotification(message, 3f);
                activated = true;
            }
        }
    }
}
