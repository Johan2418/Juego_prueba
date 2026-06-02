using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretZoneEntrance : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt = "Entrar a zona secreta";
    [SerializeField] private string secretZoneSceneName = "SecretZone_01";

    [Header("Demo Route (Optional)")]
    [SerializeField] private bool enableDemoRouteHook;
    [SerializeField] private bool overrideDefaultSceneLoadWithDemoRoute = true;
    [SerializeField] private string demoRouteInteractionId;

    public void Interact(PlayerInteractor interactor)
    {
        if (TryHandleDemoRoute(interactor, ShouldOverrideWithDemoRoute()))
        {
            return;
        }

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

    // Permite usar la roca como puerta logica de la demo sin forzar cambio de escena.
    private bool TryHandleDemoRoute(PlayerInteractor interactor, bool consumeInteraction)
    {
        if (!ShouldUseDemoRouteHook())
        {
            return false;
        }

        bool handled = DemoQuestRouteManager.Instance.TryHandleInteraction(ResolveDemoRouteInteractionId(), interactor);
        return consumeInteraction && handled;
    }

    private bool ShouldUseDemoRouteHook()
    {
        return DemoQuestRouteManager.Instance != null &&
            (enableDemoRouteHook || DemoQuestRouteManager.Instance.CanHandleInteraction(ResolveDemoRouteInteractionId()));
    }

    private bool ShouldOverrideWithDemoRoute()
    {
        return overrideDefaultSceneLoadWithDemoRoute ||
            (DemoQuestRouteManager.Instance != null &&
             DemoQuestRouteManager.Instance.CanHandleInteraction(ResolveDemoRouteInteractionId()));
    }

    private string ResolveDemoRouteInteractionId()
    {
        return string.IsNullOrWhiteSpace(demoRouteInteractionId) ? gameObject.name : demoRouteInteractionId;
    }
}
