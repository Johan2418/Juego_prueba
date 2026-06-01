using System;

[Serializable]
public class CollectItemObjective : QuestObjective
{
    public CollectItemObjective()
    {
    }

    public CollectItemObjective(string targetId, int targetAmount) : base(targetId, targetAmount)
    {
    }

    protected override bool CanHandleEvent(QuestEvent questEvent)
    {
        return questEvent.EventType == QuestEventType.ItemCollected && IsTargetMatch(questEvent.TargetId);
    }

    protected override bool ApplyProgress(QuestEvent questEvent)
    {
        return AddProgress(questEvent.Amount);
    }
}
