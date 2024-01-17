using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGoal : Quest.QuestGoal
{
    public string coin;

    public override string GetDescription()
    {
        return $"Collected {coin}";
    }

    //void Collecting()
}
