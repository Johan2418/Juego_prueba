using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcDisplayName = "NPC";
    [SerializeField] private string npcId = "npc";
    [SerializeField] private string interactionPrompt = "Presiona E para hablar";
    [SerializeField] private DialogueData defaultDialogue;
    [SerializeField] private NPCQuestGiver questGiver;

    [Header("Demo Route (Optional)")]
    [SerializeField] private bool enableDemoRouteHook;
    [SerializeField] private bool overrideDefaultInteractionWithDemoRoute;
    [SerializeField] private string demoRouteInteractionId;

    public void Interact(PlayerInteractor interactor)
    {
        if (TryHandleDemoRoute(interactor, ShouldOverrideWithDemoRoute()))
        {
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager is not present in scene.");
            return;
        }

        DialogueData selectedDialogue = questGiver != null
            ? questGiver.GetDialogueForCurrentState(defaultDialogue)
            : defaultDialogue;

        if (selectedDialogue == null)
        {
            QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked(npcId, 1));
            questGiver?.HandleAfterDialogue(interactor);
            TryHandleDemoRoute(interactor, false);
            return;
        }

        DialogueManager.Instance.StartDialogue(selectedDialogue, interactor, () =>
        {
            QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked(npcId, 1));

            if (questGiver != null)
            {
                questGiver.HandleAfterDialogue(interactor);
            }

            TryHandleDemoRoute(interactor, false);
        });
    }

    public string GetInteractionPrompt()
    {
        if (!string.IsNullOrWhiteSpace(interactionPrompt))
        {
            return interactionPrompt;
        }

        return $"Presiona E para hablar con {npcDisplayName}";
    }

    // Permite conectar la ruta de demo sin romper NPCs que siguen usando dialogo normal.
    private bool TryHandleDemoRoute(PlayerInteractor interactor, bool consumeInteraction)
    {
        if (!ShouldUseDemoRouteHook())
        {
            return false;
        }

        string interactionId = ResolveDemoRouteInteractionId();
        bool handled = DemoQuestRouteManager.Instance.TryHandleInteraction(interactionId, interactor);
        return consumeInteraction && handled;
    }

    private bool ShouldUseDemoRouteHook()
    {
        return DemoQuestRouteManager.Instance != null &&
            (enableDemoRouteHook || DemoQuestRouteManager.Instance.CanHandleInteraction(ResolveDemoRouteInteractionId()));
    }

    private bool ShouldOverrideWithDemoRoute()
    {
        return overrideDefaultInteractionWithDemoRoute ||
            (DemoQuestRouteManager.Instance != null &&
             DemoQuestRouteManager.Instance.CanHandleInteraction(ResolveDemoRouteInteractionId()));
    }

    private string ResolveDemoRouteInteractionId()
    {
        if (!string.IsNullOrWhiteSpace(demoRouteInteractionId))
        {
            return demoRouteInteractionId;
        }

        if (!string.IsNullOrWhiteSpace(gameObject.name))
        {
            return gameObject.name;
        }

        return npcId;
    }
}
