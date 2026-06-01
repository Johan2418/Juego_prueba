using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcDisplayName = "NPC";
    [SerializeField] private string npcId = "npc";
    [SerializeField] private string interactionPrompt = "Presiona E para hablar";
    [SerializeField] private DialogueData defaultDialogue;
    [SerializeField] private NPCQuestGiver questGiver;

    public void Interact(PlayerInteractor interactor)
    {
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
            return;
        }

        DialogueManager.Instance.StartDialogue(selectedDialogue, interactor, () =>
        {
            QuestManager.Instance?.SubmitEvent(QuestEvent.ForNpcTalked(npcId, 1));

            if (questGiver != null)
            {
                questGiver.HandleAfterDialogue(interactor);
            }
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
}
