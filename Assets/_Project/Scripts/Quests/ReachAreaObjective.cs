using System;

[Serializable]
public class ReachAreaObjective : QuestObjective
{
    public ReachAreaObjective()
    {
    }

    public ReachAreaObjective(string targetId, int targetAmount) : base(targetId, targetAmount)
    {
    }

    protected override bool CanHandleEvent(QuestEvent questEvent)
    {
        return questEvent.EventType == QuestEventType.AreaReached && IsTargetMatch(questEvent.TargetId);
    }

    protected override bool ApplyProgress(QuestEvent questEvent)
    {
        return AddProgress(questEvent.Amount);
    }
}
