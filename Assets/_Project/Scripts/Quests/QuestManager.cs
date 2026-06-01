using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private sealed class QuestRuntime
    {
        public QuestData Data;
        public QuestObjective Objective;
        public QuestState State;
    }

    [SerializeField] private bool logChanges = true;

    public static QuestManager Instance { get; private set; }
    public event Action<QuestData, QuestState> QuestStateChanged;

    private readonly Dictionary<string, QuestRuntime> activeQuests = new Dictionary<string, QuestRuntime>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public QuestState GetQuestState(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId) || !activeQuests.TryGetValue(questId, out QuestRuntime runtime))
        {
            return QuestState.NotStarted;
        }

        return runtime.State;
    }

    public bool StartQuest(QuestData questData)
    {
        if (questData == null || string.IsNullOrWhiteSpace(questData.QuestId))
        {
            return false;
        }

        if (activeQuests.TryGetValue(questData.QuestId, out QuestRuntime existing) && existing.State != QuestState.NotStarted)
        {
            return false;
        }

        QuestObjective objective = questData.CreateObjective();
        objective.ResetProgress();

        QuestRuntime runtime = new QuestRuntime
        {
            Data = questData,
            Objective = objective,
            State = QuestState.InProgress
        };

        activeQuests[questData.QuestId] = runtime;
        RaiseQuestStateChanged(runtime);
        return true;
    }

    public bool TurnInQuest(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId) || !activeQuests.TryGetValue(questId, out QuestRuntime runtime))
        {
            return false;
        }

        if (runtime.State != QuestState.Completed)
        {
            return false;
        }

        runtime.State = QuestState.TurnedIn;
        RaiseQuestStateChanged(runtime);
        return true;
    }

    public void SubmitEvent(QuestEvent questEvent)
    {
        List<QuestRuntime> completedQuests = null;

        foreach (KeyValuePair<string, QuestRuntime> pair in activeQuests)
        {
            QuestRuntime runtime = pair.Value;
            if (runtime.State != QuestState.InProgress || runtime.Objective == null)
            {
                continue;
            }

            if (!runtime.Objective.TryProgress(questEvent))
            {
                continue;
            }

            if (logChanges)
            {
                Debug.Log($"Quest progress updated: {runtime.Data.QuestTitle} ({runtime.Objective.CurrentAmount}/{runtime.Objective.TargetAmount})");
            }

            if (!runtime.Objective.IsComplete)
            {
                continue;
            }

            runtime.State = QuestState.Completed;
            completedQuests ??= new List<QuestRuntime>();
            completedQuests.Add(runtime);
        }

        if (completedQuests == null)
        {
            return;
        }

        for (int i = 0; i < completedQuests.Count; i++)
        {
            RaiseQuestStateChanged(completedQuests[i]);
        }
    }

    public bool IsQuestTurnedIn(string questId)
    {
        return GetQuestState(questId) == QuestState.TurnedIn;
    }

    private void RaiseQuestStateChanged(QuestRuntime runtime)
    {
        if (runtime == null || runtime.Data == null)
        {
            return;
        }

        if (logChanges)
        {
            Debug.Log($"Quest state changed: {runtime.Data.QuestTitle} -> {runtime.State}");
        }

        QuestStateChanged?.Invoke(runtime.Data, runtime.State);
    }
}
