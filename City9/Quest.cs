using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The status of a quest
/// </summary>
public enum Status
{
    inactive, active, canComplete ,completed
}

/// <summary>
/// The different quest types
/// </summary>
public enum QuestType
{
    Location, Item, Boss 
}

/// <summary>
/// Contains all reward values
/// </summary>
public struct Reward
{
    public int Xp;
    public int Money;

    public Reward(int xp, int money)
    {
        Xp = xp;
        Money = money;
    }
}

/// <summary>
/// Base class for all quests
/// </summary>
public abstract class Quest
{
    //The Id if the quest
    public int Id;

    public Status QuestStatus;

    public string Name;

    public string Description;

    //The Xp and Money reward
    public Reward reward = new Reward(100, 200);
   
    /// <summary>
    /// Is the Quest completed
    /// </summary>
    /// <returns></returns>
    public virtual bool IsCompleted()
    {
        return false;
    }

    /// <summary>
    /// Can the quest be completed
    /// </summary>
    /// <returns></returns>
    public bool CanComplete()
    {
        return QuestStatus < Status.completed;
    }

    public virtual void UpdateQuest()
    {

    }

    public virtual void CompleteQuest()
    {
        
    }
}

/// <summary>
/// Quest to kill a boss
/// </summary>
public class KillBossQuest : Quest
{
    GameObject Enemy;

    public KillBossQuest(GameObject enemy, string name, Reward questReward, int id =-1)
    {
        QuestStatus = Status.active;
        Name = name;
        Description = "";
        Enemy = enemy;
        Id = id;
        reward = questReward;
    }

    public override bool IsCompleted()
    {
        if (Enemy)
        {
            return Enemy.GetComponent<EnemyWordAI>().Stats.IsDead;
        }
        return false;
    }
}

/// <summary>
/// Quest to go to some place
/// </summary>
public class LocationQuest : Quest
{
    Vector3 position;

    float completionRange = 15;
    
    public LocationQuest(Vector3 pos, string name, Reward questReward, int id = -1)
    {
        QuestStatus = Status.active;
        Name = name;
        position = pos;
        Id = id;
        reward = questReward;

    }

    public override bool IsCompleted()
    {
        return Vector3.Distance(Motor.instance.transform.position, position) < completionRange;
    }

}

/// <summary>
/// Quest to find some item
/// </summary>
public class ItemQuest : Quest
{
    Item item;

    bool completed;

    public void SetCompleted(bool Completed)
    {
        completed = Completed;
    }

    public ItemQuest(Item i, string name, int id = -1)
    {
        item = i;
        QuestStatus = Status.active;
        Name = name;
        Id = id;
    }

    public override bool IsCompleted()
    {
        return completed;
    }

}

/// <summary>
/// Quest to talk to someone
/// </summary>
public class BoolQuest : Quest
{

    bool QuestBool = false;
    bool Completed = false;
    public BoolQuest(string name, Reward questReward, int id = -1)
    {
        QuestStatus = Status.active;
        Name = name;
        Id = id;
        reward = questReward;
    }

    public override void CompleteQuest()
    {
        //if(QuestBool)
        //{
        //    Completed = true;    
        //}
    }
    public override bool IsCompleted()
    {
        return Completed;
    }

    public override void UpdateQuest()
    {
        Completed = true;
    }
}