using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Interactuar";
    [SerializeField] private string message = "Mensaje de prueba";

    public void Interact(PlayerInteractor interactor)
    {
        // Los objetos simples de la demo pueden delegar en el manager sin cambiar el Player.
        if (DemoQuestRouteManager.Instance != null &&
            DemoQuestRouteManager.Instance.TryHandleInteraction(gameObject.name, interactor))
        {
            return;
        }

        interactor.ShowNotification(message, 3f);
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}
