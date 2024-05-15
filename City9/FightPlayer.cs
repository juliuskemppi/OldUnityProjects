using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player in a fight
/// </summary>
public class FightPlayer : Fighter
{
    Inventory i;

    bool PlayerTurn = true;

    Item itemToUse;

    public FightAction PlayerAction;

    public void SetPlayerTurn(bool turn)
    {
        PlayerTurn = turn;
    }

    //player stats
    public PlayerStatsFight playerStats = new PlayerStatsFight();


    void Start()
    {
        //Set the ui component
        FighterUI = StatsPanel.GetComponent<FightStatsUI>();

        //Set the animation to fight idle
        GetComponentInChildren<Animator>().SetBool("fighting", true);

        //Get the inventory
        i = GetComponent<Inventory>();
        i.BuildFromString(PlayerPrefs.GetString("Inventory"));
        i.SetEquippedItems(PlayerPrefs.GetString("UsedItems"));

        //Update defend chance
        //GetStats();vc
        Invoke("GetStats",0.1f);
    }

    private void OnDestroy()
    {
        GetComponentInChildren<Animator>().SetBool("fighting", false);
    }

    void GetStats()
    {
        playerStats.MaxHealth = PlayerPrefs.GetInt("PlayerMaxHealth");
        playerStats.PlayerHealth = PlayerPrefs.GetInt("PlayerHealth");
        playerStats.PlayerDamage = PlayerPrefs.GetInt("PlayerDamage");
        if (i.GetShield())
        {
            defendChance = i.GetShield().defendChance;
            CounterChance += i.GetShield().counterChance;
        } 
        //try
        //{
        //    playerStats.PlayerDamage += i.GetWeaponDamage();
        //}
        //catch
        //{

        //}
        playerStats.PlayerArmor = i.GetTotalArmor();
        //playerStats.PlayerWeapon = i.GetWeapon();
    }

    public void Attack()
    {
        if (PlayerTurn)
        {
            PlayerAction = FightAction.Attack;
            EndTurn();
        }
    }

    public void Defend()
    {
        if (PlayerTurn)
        {
            PlayerAction = FightAction.Defend;
            EndTurn();
        }
    }

    public void ItemTurn()
    {
        if(PlayerTurn)
        {
            PlayerAction = FightAction.UseItem;
            EndTurn();
        }
    }

    /// <summary>
    /// Makes the player run from the fight
    /// </summary>
    public void Run()
    {
        if (PlayerTurn)
        {
            if (Random.Range(0f, 1f) < 0.35)
            {
                PlayerPrefs.SetInt("PlayerHealth", playerStats.PlayerHealth);
                PlayerPrefs.SetString("Inventory", i.InventoryToString());
                SceneManager.instance.ChangeScene(PlayerPrefs.GetString("SceneFight"));
            }
            else
            {
                PlayerAction = FightAction.None;
                EndTurn();
            }
        }
    }

    void EndTurn()
    {
        Debug.Log.Add(PlayerAction.ToString());
        FightManager.Instance.EndTurn("Player");
    }

    public int GetDamage()
    {
        return playerStats.PlayerDamage;
    }

    public float GetAP()
    {
        return playerStats.PlayerWeapon.ArmorPierce;
    }

    public void TakeDamage(int DamageAmount)
    {
        playerStats.PlayerHealth -= DamageAmount;
        FighterUI.ShowDamage(DamageAmount);
        transform.GetChild(0).GetChild(0).GetComponent<PlayerAnimationControl>().SetAction(3);
        FightManager.Instance.TryEnd();
    }

    public void UseItem()
    {
        itemToUse.Use();
    }

    
    public void SetItemToUse(Item item)
    {
        itemToUse = item;
        ItemTurn();
    }

    public string GetItemName()
    {
        return itemToUse.DisplayName;
    }
}

[System.Serializable]
public class PlayerStatsFight
{
    public int MaxHealth = 100;
    public int PlayerHealth = 100;
    public int PlayerDamage = 10;
    public int PlayerArmor = 1;
    public WeaponItem PlayerWeapon;
}
