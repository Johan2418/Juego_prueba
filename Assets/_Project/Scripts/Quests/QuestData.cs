using UnityEngine;

public enum QuestObjectiveType
{
    CatchFish = 0,
    CollectItem = 1,
    TalkToNPC = 2,
    ReachArea = 3
}

[CreateAssetMenu(menuName = "CozyGameplay/Quests/Quest Data", fileName = "QuestData")]
public class QuestData : ScriptableObject
{
    [SerializeField] private string questId;
    [SerializeField] private string questTitle;
    [TextArea(2, 5)]
    [SerializeField] private string questDescription;
    [SerializeField] private QuestObjectiveType objectiveType = QuestObjectiveType.CatchFish;
    [SerializeField] private string targetId;
    [SerializeField] private int targetAmount = 1;

    public string QuestId => questId;
    public string QuestTitle => string.IsNullOrWhiteSpace(questTitle) ? name : questTitle;
    public string QuestDescription => questDescription;
    public QuestObjectiveType ObjectiveType => objectiveType;
    public string TargetId => targetId;
    public int TargetAmount => Mathf.Max(1, targetAmount);

    public QuestObjective CreateObjective()
    {
        switch (objectiveType)
        {
            case QuestObjectiveType.CatchFish:
                return new CatchFishObjective(targetId, targetAmount);
            case QuestObjectiveType.CollectItem:
                return new CollectItemObjective(targetId, targetAmount);
            case QuestObjectiveType.TalkToNPC:
                return new TalkToNPCObjective(targetId, targetAmount);
            case QuestObjectiveType.ReachArea:
                return new ReachAreaObjective(targetId, targetAmount);
            default:
                return new CatchFishObjective(targetId, targetAmount);
        }
    }

    private void OnValidate()
    {
        targetAmount = Mathf.Max(1, targetAmount);

        if (string.IsNullOrWhiteSpace(questId))
        {
            questId = name.ToLowerInvariant().Replace(" ", "_");
        }
    }
}
