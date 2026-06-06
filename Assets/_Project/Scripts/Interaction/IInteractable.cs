public interface IInteractable
{
    void Interact(PlayerInteractor interactor);
    string GetInteractionPrompt();
}

public interface IInteractionFocusListener
{
    void OnInteractionFocusEntered(PlayerInteractor interactor);
    void OnInteractionFocusExited(PlayerInteractor interactor);
}
