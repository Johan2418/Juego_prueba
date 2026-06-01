using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{
    [SerializeField] private string npcId = "npc_fisher";
    [SerializeField] private QuestData questToGive;
    [SerializeField] private DialogueData beforeQuestDialogue;
    [SerializeField] private DialogueData inProgressDialogue;
    [SerializeField] private DialogueData completedDialogue;
    [SerializeField] private DialogueData turnedInDialogue;

    public string NpcId => npcId;
    public QuestData QuestToGive => questToGive;

    public DialogueData GetDialogueForCurrentState(DialogueData fallback)
    {
        if (questToGive == null || QuestManager.Instance == null)
        {
            return fallback;
        }

        QuestState state = QuestManager.Instance.GetQuestState(questToGive.QuestId);
        switch (state)
        {
            case QuestState.NotStarted:
                return beforeQuestDialogue != null ? beforeQuestDialogue : fallback;
            case QuestState.InProgress:
                return inProgressDialogue != null ? inProgressDialogue : fallback;
            case QuestState.Completed:
                return completedDialogue != null ? completedDialogue : fallback;
            case QuestState.TurnedIn:
                return turnedInDialogue != null ? turnedInDialogue : fallback;
            default:
                return fallback;
        }
    }

    public void HandleAfterDialogue(PlayerInteractor interactor)
    {
        if (QuestManager.Instance == null)
        {
            return;
        }

        if (questToGive == null)
        {
            return;
        }

        QuestState state = QuestManager.Instance.GetQuestState(questToGive.QuestId);

        if (state == QuestState.NotStarted && QuestManager.Instance.StartQuest(questToGive))
        {
            interactor?.ShowNotification($"Mision iniciada: {questToGive.QuestTitle}", 2.5f);
            return;
        }

        if (state == QuestState.Completed && QuestManager.Instance.TurnInQuest(questToGive.QuestId))
        {
            interactor?.ShowNotification($"Mision entregada: {questToGive.QuestTitle}", 2.5f);
        }
    }
}
