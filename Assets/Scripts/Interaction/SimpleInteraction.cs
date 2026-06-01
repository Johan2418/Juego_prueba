using UnityEngine;

public class SimpleInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Interactuar";
    [SerializeField] private string message = "Mensaje de prueba";

    public void Interact(PlayerInteractor interactor)
    {
        interactor.ShowNotification(message, 3f);
    }

    public string GetInteractionPrompt()
    {
        return prompt;
    }
}
