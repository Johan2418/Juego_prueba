using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretZoneEntrance : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Entrar a zona secreta";
    [SerializeField] private string secretZoneSceneName = "SecretZone_01";

    public void Interact(PlayerInteractor interactor)
    {
        if (string.IsNullOrWhiteSpace(secretZoneSceneName))
        {
            Notify(interactor, "Escena de zona secreta no configurada.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(secretZoneSceneName))
        {
            string message = $"La escena '{secretZoneSceneName}' no existe en Build Settings todavia.";
            Debug.LogWarning(message);
            Notify(interactor, message);
            return;
        }

        SceneManager.LoadScene(secretZoneSceneName);
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    private static void Notify(PlayerInteractor interactor, string message)
    {
        interactor?.ShowNotification(message, 2.5f);
    }
}
