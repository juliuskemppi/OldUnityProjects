using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


// 2024 I'm pretty sure I had no idea what i was doing.
/// <summary>
/// The Enemy AI used in the fight scene
/// </summary>
public class EnemyFightAI : Fighter
{
    //The stats of the player
    PlayerStatsFight PlayerStats;

    FightPlayer player;

    //Enemy stats
    public EnemyStatsFight Stats = new EnemyStatsFight();

    public bool attack;

    List<Action> Actions = new List<Action>(); //2024 this isnt used anywhere should be removed

    public InventorySlot ItemSlot;

    int SeesPlayer = -1;

    int difficultyLevel;

    [SerializeField]
    GameObject PoisonText;

    /// <summary>
    /// Sets the difficulty level of the enemy
    /// </summary>
    /// <param name="level"></param>
    public void SetDifficulty(int level)
    {
        difficultyLevel = level;
    }

    /// <summary>
    /// Gets the action base on the priority
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
    Action GetPriorityAction(int priority) //2024 I have no idea what this function is supposed to work, it isn't even used. why wasn't this removed
    {
        return Attack;
        //If the priority is the lowest
        if(priority <= 3)
        {
            if(PlayerAttack())
            {
                if (!CanSurvive())
                {
                    if (!CanKill())
                    {
                        return Attack;
                    }
                    else
                    {
                        return Defend;
                    }
                }
                else if(CanKill())
                {
                    return Defend;
                }
                else if(HasItems())
                {
                    if(!ShouldHeal())
                    return UseItem;
                }
                return Attack;
            }
            else
            {
                return Defend;
            }
        }

        //If the priority is middle
        else if(priority == 2)
        {
            if(SeesPlayer <= 2)
            {
                if(PlayerAttack())
                {
                    if(!CanSurvive())
                    {
                        if (HasItems())
                        {
                            if (ShouldHeal())
                            {
                                return Defend;
                            }
                            else
                            {
                                return UseItem;
                            }
                        }
                    }
                    else if(CanKill())
                    {
                        if(ShouldHeal())
                        {
                           return Defend;
                        }
                        return UseItem;
                    }
                }
                else
                {
                    if(!CanSurvive())
                    {
                        if(HasItems())
                        {
                            if (ShouldHeal())
                            {
                                return Attack;
                            }
                            else return UseItem;
                        }
                        return Defend;
                    }
                }
            }
        }

        //If the priority is highest
        else if (priority == 1)
        {
            if (CanKill())
            {
                return Attack;
            }
            if (SeesPlayer == 1)
            {
                if (PlayerAttack())
                {
                    if (!CanSurvive(1, true))
                    {
                        if (HasItems())
                        {
                            if (ShouldHeal())
                                return UseItem;
                        }
                        return Defend;
                    }
                }    
                else
                {
                    return Attack;
                }
            }
            else
            {
                if(!CanSurvive())
                {
                    if(HasItems())
                    {
                        if(ShouldHeal())
                        {
                            return UseItem;
                        }
                    }
                    return Defend;
                }
            }
        }

        return Attack;
    }

    [SerializeField]Item item;

    [SerializeField]
    float BadChance = 0.0f;

    //Poison variables
    bool Poisoned = false;
    int poisonDamage;
    int poisonDuration;

    public FightAction EnemyAction;

    // Use this for initialization
    void Start()
    {
        FighterUI = StatsPanel.GetComponent<FightStatsUI>();
        Stats.EnemyDamage = PlayerPrefs.GetInt("EnemyDamage");
        Stats.EnemyHealth = PlayerPrefs.GetInt("EnemyHealth");
        Stats.EnemyArmor = PlayerPrefs.GetInt("EnemyArmor");
        SetDifficulty(3);
        player = FightManager.Instance.Player;

        int type = PlayerPrefs.GetInt("EnemyType");
        if(type == 0)
        {
            transform.GetChild(0).Find("Rat").gameObject.SetActive(true);
        }
        else if(type == 1)
        {
            transform.GetChild(0).Find("BigRat").gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Should the AI heal
    /// </summary>
    /// <returns></returns>
    bool ShouldHeal()
    {
        HealthPotion a = item as HealthPotion;
        if (a != null)
        {
            if (a.HealAmount > FightManager.GetReducedDamage(PlayerStats.PlayerDamage, Stats.EnemyArmor))
            {
                return true;
            }
        }
        return false;
    }

    int CountItemPriority()
    {
        int tempPriority = 3;
        if (HasItems())
        {
            HealthPotion a = item as HealthPotion;
            if (a != null)
            {
                if (a.HealAmount > FightManager.GetReducedDamage(PlayerStats.PlayerDamage, Stats.EnemyArmor))
                {
                    tempPriority = -1;

                    if (CanKill(2) && CanSurvive(2))
                    {
                        tempPriority = 1;
                    }
                    else if (CanKill(2) && !CanSurvive(2))
                    {
                        tempPriority = -0;
                    }
                }
                else
                {
                    tempPriority = 2;
                }
            }
        }
        return tempPriority;
    }

    /// <summary>
    /// Select the priority of the enemys attack
    /// </summary>
    void SelectAction()
    {
        int actionNum = 0;
        if (UnityEngine.Random.Range(0.02f, 1f) < BadChance)
        {
            BadChance *= .5f;
            if (UnityEngine.Random.Range(0.02f, 1f) < BadChance)
            {
                actionNum = 3;
            }
            else
            {
                actionNum = 2;
            }
        }
        else
        {
            actionNum = 1;
        }
        Debug.Log.Add("Priority: " + actionNum.ToString());
        DoAction(actionNum);

        //Debug.Log.Add(BadChance.ToString());
    }

    /// <summary>
    /// Does an action
    /// </summary>
    /// <param name="priority"></param>
    void DoAction(int priority)
    {
        //Can the enemy kill the player or is there no chance for the enemy to win
        if(CanKill() || (!CanSurvive() && !CanKill(4)))
        {
            //Always attack
            Attack();
        }
        else
        {
            //Do things
            HardAction(priority);
        }
    }

    /// <summary>
    /// Actions when ai difficulty is easy
    /// </summary>
    /// <param name="priority">how important the action is</param>
    void EasyAction(int priority)
    {
        if(!SurviveTest(priority))
        {
            Attack();
        }
       
    }

    /// <summary>
    /// Actions when ai difficulty is normal
    /// </summary>
    /// <param name="priority">how important the action is</param>
    void NormalAction(int priority)
    {
        if (!SurviveTest(priority))
        {
            Attack();
        }
    }

    /// <summary>
    /// Actions when ai difficulty is hard
    /// </summary>
    /// <param name="priority">how important the action is</param>
    void HardAction(int priority)
    {
        Debug.Log.Add("hi");
        if(UnityEngine.Random.Range(0f,1f) <= 0.5f)
        {
            if (player.PlayerAction == FightAction.Attack)
            {
                if (!SurviveTest(priority))
                {
                    Attack();
                }
            }
            else if (player.PlayerAction == FightAction.Defend)
            {
                switch (priority)
                {
                    case 1:
                        if (player.CounterChance > 0.33f)
                        {
                            UseItem();
                        }
                        else
                        {
                            Attack();
                        }
                        break;
                    case 2:

                        if (player.CounterChance > 0.33f)
                        {
                            Attack();
                        }
                        else
                        {
                            UseItem();
                        }
                        break;
                    case 3:
                        Defend();
                        break;
                }
            }
            else
            {
                switch (priority)
                {
                    case 1:
                        Attack();
                        break;
                    case 2:
                        UseItem();
                        break;
                    case 3:
                        Defend();
                        break;

                }
            }
        }
        else
        {
            Attack();
        }
    }

    /// <summary>
    /// Tests stuff about surviving
    /// </summary>
    bool SurviveTest(int priority)
    {
        if (!CanSurvive() && !CanKill())
        {
            if (ShouldHeal())
            {
                switch (priority)
                {
                    case 1:
                        UseItem();
                        break;
                    case 2:
                        Defend();
                        break;
                    case 3:
                        Attack();
                        break;
                    default:
                        break;
                }
                return true;
            }
            else
            {
                switch (priority)
                {
                    case 1:
                        Defend();
                        break;
                    case 2:
                        UseItem();
                        break;
                    case 3:
                        Attack();
                        break;
                    default:
                        break;
                }
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the players stats
    /// </summary>
    void UpdatePlayerStats()
    {
        PlayerStats = FightManager.Instance.Player.playerStats;
    }

    /// <summary>
    /// Starts the turn of the AI enemy
    /// </summary>
    public void StartTurn()
    {
        if(Poisoned)
        {
            TakeDamage(poisonDamage);
            poisonDuration--;
            if(poisonDuration <=0)
            {
                Poisoned = false;
                PoisonText.SetActive(false);   
            }
        }
        UpdatePlayerStats();
        SelectAction();
    }
    
    /// <summary>
    /// Tests if the AI has items
    /// </summary>
    /// <returns></returns>
    bool HasItems()
    {
        return ItemSlot != null && ItemSlot.GetAmount() > 0;
    }

    /// <summary>
    /// Tests if the AI can survive for given amount of turns
    /// </summary>
    /// <param name="turns"> The amount of turns</param>
    /// <param name="accurate"> is the calculation accurate or safe</param>
    /// <returns></returns>
    bool CanSurvive(int turns = 1, bool accurate = false)
    {
        if (!accurate)
        {
            return Stats.EnemyHealth - turns * PlayerStats.PlayerDamage > 5;
        }
        else
        {
            return Stats.EnemyHealth - turns * FightManager.GetReducedDamage(PlayerStats.PlayerDamage, Stats.EnemyArmor) > 0;
        }
    }

    /// <summary>
    /// Can the AI kill the player in turns turns
    /// </summary>
    /// <param name="turns"></param>
    /// <returns></returns>
    bool CanKill(int turns = 1)
    {
        return PlayerStats.PlayerHealth - FightManager.GetReducedDamage(turns * Stats.EnemyDamage,PlayerStats.PlayerArmor) <= 0;
    }

    /// <summary>
    /// Ends the turn by attacking
    /// </summary>
    void Attack()
    {
        EnemyAction = FightAction.Attack;
        EndTurn();
    }


    /// <summary>
    /// Ends the turn by defending
    /// </summary>
    void Defend()
    {
        EnemyAction = FightAction.Defend;
        EndTurn();
    }

    /// <summary>
    /// Ends the turn by using an item
    /// </summary>
    void UseItem()
    {
        //Debug.Log.Add("Item Used");
        EnemyAction = FightAction.UseItem;
        EndTurn();
    }

    public void UseItemEnd()
    {
        Stats.EnemyHealth += 5;
    }

    /// <summary>
    /// Tells the fight manager to end the turn
    /// </summary>
    void EndTurn()
    {
        FightManager.Instance.EndTurn("Enemy");
    }

    /// <summary>
    /// Enemy takes damage
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        //Debug.Log.Add(damage.ToString());
        Stats.EnemyHealth -= damage;
        FighterUI.ShowDamage(damage);
        //transform.GetChild(0).GetChild(0).GetComponent<PlayerAnimationControl>().SetAction(3);
        FightManager.Instance.TryEnd();
        //Debug.Log.Add(Stats.EnemyHealth.ToString());
    }

    /// <summary>
    /// Starts the delayed turn of the enemy
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator TurnTime(float time)
    {
        yield return new WaitForSeconds(time);
        StartTurn();   
    }

    /// <summary>
    /// See did the player attack this turn
    /// </summary>
    /// <returns></returns>
    bool PlayerAttack()
    {
        return false;
    }

    /// <summary>
    /// Poison the enemy
    /// </summary>
    /// <param name="damage">How much damage does the poison do in a turn</param>
    /// <param name="duration">How many turns the poison lasts</param>
    public void Poison(int damage, int duration =2)
    {
        Poisoned = true;
        poisonDamage = damage;
        poisonDuration = duration;
        PoisonText.SetActive(true);
    }

    
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class EnemyStatsFight
{
    public int EnemyHealth;
    public int EnemyDamage;
    public int EnemyArmor;
}
