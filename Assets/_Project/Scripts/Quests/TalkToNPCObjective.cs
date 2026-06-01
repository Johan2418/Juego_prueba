using System;

[Serializable]
public class TalkToNPCObjective : QuestObjective
{
    public TalkToNPCObjective()
    {
    }

    public TalkToNPCObjective(string targetId, int targetAmount) : base(targetId, targetAmount)
    {
    }

    protected override bool CanHandleEvent(QuestEvent questEvent)
    {
        return questEvent.EventType == QuestEventType.NpcTalked && IsTargetMatch(questEvent.TargetId);
    }

    protected override bool ApplyProgress(QuestEvent questEvent)
    {
        return AddProgress(questEvent.Amount);
    }
}
