using UnityEngine;

public class CulturalInspectable : MonoBehaviour, IInteractable, IInteractionFocusListener
{
    [SerializeField] private string interactionPrompt = "Presione el boton E para inspeccionar";
    [SerializeField, TextArea(3, 6)] private string description =
        "La silla manteña (o silla en \"U\") es un ícono ceremonial y arqueológico de la Cultura Manteña. " +
        "Símbolo de identidad y poder de Manabí, representa la jerarquía política, religiosa y chamánica " +
        "de sus antiguos habitantes.";

    private bool descriptionVisible;

    public void Configure(string prompt, string culturalDescription)
    {
        interactionPrompt = prompt;
        description = culturalDescription;
    }

    public void Interact(PlayerInteractor interactor)
    {
        descriptionVisible = true;
        interactor?.ShowPersistentNotification(description);
    }

    public string GetInteractionPrompt()
    {
        return descriptionVisible ? string.Empty : interactionPrompt;
    }

    public void OnInteractionFocusEntered(PlayerInteractor interactor)
    {
        descriptionVisible = false;
    }

    public void OnInteractionFocusExited(PlayerInteractor interactor)
    {
        if (!descriptionVisible)
        {
            return;
        }

        descriptionVisible = false;
        interactor?.HideNotification();
    }
}
