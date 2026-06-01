using System;

[Serializable]
public class CatchFishObjective : QuestObjective
{
    public CatchFishObjective()
    {
    }

    public CatchFishObjective(string targetId, int targetAmount) : base(targetId, targetAmount)
    {
    }

    protected override bool CanHandleEvent(QuestEvent questEvent)
    {
        return questEvent.EventType == QuestEventType.FishCaught && IsTargetMatch(questEvent.TargetId);
    }

    protected override bool ApplyProgress(QuestEvent questEvent)
    {
        return AddProgress(questEvent.Amount);
    }
}
