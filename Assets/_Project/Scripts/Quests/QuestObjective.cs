using System;
using UnityEngine;

public enum QuestEventType
{
    FishCaught = 0,
    ItemCollected = 1,
    NpcTalked = 2,
    AreaReached = 3
}

public readonly struct QuestEvent
{
    public QuestEventType EventType { get; }
    public string TargetId { get; }
    public int Amount { get; }

    public QuestEvent(QuestEventType eventType, string targetId, int amount = 1)
    {
        EventType = eventType;
        TargetId = targetId;
        Amount = Mathf.Max(1, amount);
    }

    public static QuestEvent ForFishCaught(string fishId, int amount = 1)
    {
        return new QuestEvent(QuestEventType.FishCaught, fishId, amount);
    }

    public static QuestEvent ForItemCollected(string itemId, int amount = 1)
    {
        return new QuestEvent(QuestEventType.ItemCollected, itemId, amount);
    }

    public static QuestEvent ForNpcTalked(string npcId, int amount = 1)
    {
        return new QuestEvent(QuestEventType.NpcTalked, npcId, amount);
    }

    public static QuestEvent ForAreaReached(string areaId, int amount = 1)
    {
        return new QuestEvent(QuestEventType.AreaReached, areaId, amount);
    }
}

[Serializable]
public abstract class QuestObjective
{
    [SerializeField] private string targetId;
    [SerializeField] private int targetAmount = 1;

    public string TargetId => targetId;
    public int TargetAmount => Mathf.Max(1, targetAmount);
    public int CurrentAmount { get; private set; }
    public bool IsComplete => CurrentAmount >= TargetAmount;

    protected QuestObjective()
    {
        targetId = string.Empty;
        targetAmount = 1;
    }

    protected QuestObjective(string targetId, int targetAmount)
    {
        this.targetId = targetId;
        this.targetAmount = Mathf.Max(1, targetAmount);
    }

    public void ResetProgress()
    {
        CurrentAmount = 0;
    }

    public bool TryProgress(QuestEvent questEvent)
    {
        if (IsComplete || !CanHandleEvent(questEvent))
        {
            return false;
        }

        return ApplyProgress(questEvent);
    }

    protected bool IsTargetMatch(string incomingTargetId)
    {
        if (string.IsNullOrWhiteSpace(targetId))
        {
            return true;
        }

        return string.Equals(targetId, incomingTargetId, StringComparison.OrdinalIgnoreCase);
    }

    protected bool AddProgress(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        int previousAmount = CurrentAmount;
        CurrentAmount = Mathf.Min(TargetAmount, CurrentAmount + amount);
        return CurrentAmount > previousAmount;
    }

    protected abstract bool CanHandleEvent(QuestEvent questEvent);
    protected abstract bool ApplyProgress(QuestEvent questEvent);
}
