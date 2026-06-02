using UnityEngine;

public class DemoRouteInteractionPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Presiona E para interactuar";
    [SerializeField] private string demoRouteInteractionId;
    [SerializeField] private string fallbackMessage = "No hay reaccion de demo configurada.";

    public void Interact(PlayerInteractor interactor)
    {
        if (DemoQuestRouteManager.Instance != null &&
            DemoQuestRouteManager.Instance.TryHandleInteraction(ResolveDemoRouteInteractionId(), interactor))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(fallbackMessage))
        {
            interactor?.ShowNotification(fallbackMessage, 2.5f);
            Debug.Log($"[DemoRouteInteractionPoint] {fallbackMessage}");
        }
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    private string ResolveDemoRouteInteractionId()
    {
        return string.IsNullOrWhiteSpace(demoRouteInteractionId) ? gameObject.name : demoRouteInteractionId;
    }
}
