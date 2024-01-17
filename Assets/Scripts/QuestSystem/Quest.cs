using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Quest : ScriptableObject
{
    [System.Serializable]
    public struct Info
    {
        public string name;
        public Sprite icon;
        public string description;
    }

    [Header("Info")] public Info information;

    [System.Serializable]
    public struct Stat
    {
        public int currency;
        public int xp;
    }

    [Header("Reward")] public Stat reward = new Stat { currency = 10, xp = 10 };

    public bool Completed { get; protected set; }
    public QuestCompletedEvent questCompleted;

    public abstract class QuestGoal : ScriptableObject
    {
        protected string description;
        public int CurrentAmount { get; protected set; }
        public int requiredAmount = 1;

        public bool Completed { get; protected set; }
        [HideInInspector] public UnityEvent goalCompleted;

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual void Initialize()
        {
            Completed = false;
            goalCompleted = new UnityEvent();
        }

        protected void Evaluate()
        {
            if(CurrentAmount >= requiredAmount)
            {
                Complete();
            }
        }

        void Complete()
        {
            Completed = true;
            goalCompleted.Invoke();
            goalCompleted.RemoveAllListeners();
        }

        public void Skip()
        {
            Complete();
        }
    }

    public List<QuestGoal> goals;

    public void Initialize()
    {
        Completed = false;
        questCompleted = new QuestCompletedEvent();

        foreach(var goal in goals)
        {
            goal.Initialize();
            goal.goalCompleted.AddListener(delegate { CheckGoals(); });
        }
    }

    void CheckGoals()
    {
        Completed = goals.All(g => g.Completed);
        if(Completed)
        {
            // give reward
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
        }
    }
}

public class QuestCompletedEvent : UnityEvent<Quest> { }
