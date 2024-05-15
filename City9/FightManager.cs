using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Actions that canbe done
/// </summary>
public enum FightAction
{
    None,
    Attack,
    Defend,
    UseItem
}

/// <summary>
///Manages all fight related stuff
/// </summary>
public class FightManager : MonoBehaviour
{
    //The player
    [SerializeField]
    public FightPlayer Player;

    //The enemy
    [SerializeField]
    public EnemyFightAI Enemy;

    //The panel that shows actions to the player
    [SerializeField]
    GameObject basicPanel;

    //The canvas controller script
    [SerializeField]
    FightCanvasController canvasController;

    //Is it the players turn
    public bool PlayerTurn;

    //Is players turn finished
    bool PlayerTurnFinished;

    //Is ai turn finished
    bool AITurnFinished;

    //Instance
    public static FightManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Set the players turn
        PlayerTurn = true;
        Player.SetPlayerTurn(true);
    }

    /// <summary>
    /// Gets damage reduced by armor
    /// </summary>
    /// <param name="damage">damage</param>
    /// <param name="Armor">armor</param>
    /// <returns></returns>
    public static int GetReducedDamage(int damage, int Armor)
    {
        //Clamp the damage to not go under 0
        int thing = Mathf.Clamp(damage - (Armor / 5), 0, 100000);
        return thing;
    }

    public static int GetReducedDamage(int damage, float AP, int Armor)
    {
        float trueDamage = damage * AP;
        int Damage = damage - (int)trueDamage;
        return Mathf.Clamp(Damage - (Armor / 5) + (int)trueDamage, 0, 100000);
    }

    /// <summary>
    /// Gets the how succesful defend id
    /// </summary>
    /// <param name="defendChance">Chance to defend</param>
    /// <param name="counterChance">Chance to do a counterattck</param>
    /// <returns></returns>
    public static int DefendSuccess(float defendChance, float counterChance = 0)
    {
        //Get a random float
        float Success = Random.Range(0f, 1f);
        if (Success < counterChance)
        {
            return 2;
        }
        else if(Success < defendChance)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Ends the turn for the side
    /// </summary>
    /// <param name="source">The name of the source</param>
    public void EndTurn(string source)
    {
        Debug.Log.Add("Turn ended");
        //End the turn of one side

        //Is the source enemy
        if (source == "Enemy")
        {
            Debug.Log.Add("Enemy Turn ended");

            //Set the ai turn finished
            AITurnFinished = true;
        }
        //Is the source player
        else if (source == "Player")
        {
            Debug.Log.Add("Player Turn ended");

            //Set player turn finished
            PlayerTurnFinished = true;
        }

        //Apply both turns

        //Have both player and ai finished their turns
        if (PlayerTurnFinished && AITurnFinished)
        {
            Debug.Log.Add("Turns finished");

            //Apply the enemy turn
            StartCoroutine(DoEnemyTurn());

            Debug.Log.Add("Enemy Turn applied");

            //Apply the player turn
            StartCoroutine(DoPlayerTurn());

            Debug.Log.Add("Player Turn applied");

            //Reset the turn
            AITurnFinished = false;
            PlayerTurnFinished = false;
            Debug.Log.Add("Turns reset");
            StartCoroutine(ToggleBasicPanel(true, 1f));

        }

        //Start the turn of the other side


        //Set the player turn to be not player turn
        PlayerTurn = !PlayerTurn;

        //Set the players turn
        Player.SetPlayerTurn(PlayerTurn);

        Debug.Log.Add("Set player turn");

        //If its not the players turn
        if (!PlayerTurn)
        {
            //Close the players action panel
            StartCoroutine(ToggleBasicPanel(false,0));
            //Start the enemys tuen
            StartCoroutine(Enemy.TurnTime(1.1f));
            Debug.Log.Add("Started enemy turn");

        }

    }

    /// <summary>
    /// Test if one of the fighters are dead
    /// </summary>
    public void TryEnd()
    {
        //Did either the enemy or the player die
        if (Enemy.Stats.EnemyHealth <= 0 || Player.playerStats.PlayerHealth <= 0)
        {

            //Save the player and enemy health
            PlayerPrefs.SetInt("PlayerHealth", Player.playerStats.PlayerHealth);
            PlayerPrefs.SetInt("EnemyHealth", Enemy.Stats.EnemyHealth);
            PlayerPrefs.SetString("Inventory", Inventory.instance.InventoryToString());


            //If the player died reset the games
            if (Player.playerStats.PlayerHealth <= 0)
            {
                PlayerPrefs.SetInt("Position", 0);
                SceneManager.instance.ChangeScene("Main");
            }
            //Else return to where the fight was started
            else
            {
                SceneManager.instance.ChangeScene(PlayerPrefs.GetString("SceneFight"));
            }
        }
    }

    /// <summary>
    /// Shows a text in fight scene
    /// </summary>
    /// <param name="text">text to show</param>
    void ShowText(string text, Fighter fighter)
    {
        var p = fighter.StatsPanel.GetComponent<FightStatsUI>();
        p.ShowActionText(ref text);
    }

    /// <summary>
    /// Does the enemy turn
    /// </summary>
    IEnumerator DoEnemyTurn()
    {
        //Is the enemy attacking
        if (Enemy.EnemyAction == FightAction.Attack)
        {
            ShowText("Vihollinen Iski", Enemy);
            //Is the player defending
            if (Player.PlayerAction == FightAction.Defend)
            {
                Player.transform.GetChild(0).GetChild(0).GetComponent<PlayerAnimationControl>().SetAction(2);
                //is the defense successful
                switch (DefendSuccess(Player.defendChance, Player.CounterChance))
                {
                    case 0:
                        Player.TakeDamage(GetReducedDamage(Enemy.Stats.EnemyDamage, Player.playerStats.PlayerArmor));
                        yield return new WaitForSeconds(0.8f);
                        break;
                    case 1:
                        ShowText("Pelaaja Puolusti", Player);
                        break;
                    case 2:
                        Enemy.TakeDamage(GetReducedDamage(Player.playerStats.PlayerDamage / 2, Enemy.Stats.EnemyArmor));

                        ShowText("Pelaaja Teki Vastaiskun", Player);
                        break;
                }
            }
            else
            {
                //Take damage
                Player.TakeDamage(GetReducedDamage(Enemy.Stats.EnemyDamage, Player.playerStats.PlayerArmor));
            }
        }
        //Is the enemy using an item
        else if (Enemy.EnemyAction == FightAction.UseItem)
        {
            ShowText("Vihollinen joi terveysjuoman", Enemy);

            Enemy.UseItemEnd();
        }
    }

    /// <summary>
    /// Does the players turn
    /// </summary>
    IEnumerator DoPlayerTurn()
    {
        if (Player.PlayerAction == FightAction.Attack)
        {
            ShowText("Pelaaja Iski", Player);
            Player.transform.GetChild(0).GetChild(0).GetComponent<PlayerAnimationControl>().SetAction(1);
            yield return new WaitForSeconds(0.7f);
            if (Enemy.EnemyAction == FightAction.Defend)
            {
                //is the defense successful
                switch (DefendSuccess(Enemy.defendChance, 0.2f))
                {
                    case 0:
                        Enemy.TakeDamage(GetReducedDamage(Player.playerStats.PlayerDamage, Enemy.Stats.EnemyArmor));
                        break;
                    case 1:
                        ShowText("Vihollinen Puolusti", Enemy);

                        break;
                    case 2:
                        Player.TakeDamage(GetReducedDamage(Enemy.Stats.EnemyDamage / 2, Player.playerStats.PlayerArmor));
                        ShowText("Vihollinen Teki Vastaiskun", Enemy);
                        break;
                }
            }
            else
            {
                Enemy.TakeDamage(GetReducedDamage(Player.GetDamage(), Enemy.Stats.EnemyArmor));

            }
        }
        else if (Player.PlayerAction == FightAction.UseItem)
        {
            if(Player.GetItemName() == "Myrkky")
            {
                ShowText("Pelaaja Kulutti myrkyn", Player);
            }
            else ShowText("Pelaaja Kulutti " + Player.GetItemName() + "n", Player);
            Player.UseItem();
        }
    }

    /// <summary>
    /// Toggles the basic panel
    /// </summary>
    /// <param name="isOn">Is the panel turned on</param>
    /// <param name="delay">the delay of turing it on</param>
    /// <returns></returns>
    IEnumerator ToggleBasicPanel(bool isOn, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log.Add("Basic panel set " + (isOn ? "Active" : "Not Active"));
        basicPanel.SetActive(isOn);
    }
}
