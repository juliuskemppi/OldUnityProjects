using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// The AI the enemies use in the main game world
/// </summary>
public class EnemyWordAI : MonoBehaviour
{

    [SerializeField]
    GameObject Player; //The player

    [SerializeField]
    bool isQuestRat;

    [SerializeField]
    public float outerRadius; //The Radius of the find player thing

    [SerializeField]
    float innerRadius; //Radius of start fight

    bool Chasing; //Is the enemy chasing the player

    NavMeshAgent agent; //The navigation agent in this enemy

    float timer; //A timer

    int CurTarget = 0; // The move target thing

    [SerializeField]
    Vector3[] MovePositions; //All of the move positions

    Vector3 movePosition; //The position the enemy is moving towards

    //Variables for killing and respawning enemies

    public float DeathTimer; 

    Vector3 SpawnPoint;

    //Enemy stats
    public EnemyStats Stats = new EnemyStats();
    public EnemyStats DefaultStats;

    Coroutine MainTick;

    AudioSource audio;
    AudioSource walk;

    // Use this for initialization
    void Start()
    {
        Invoke("start", 0.2f);
        AudioSource[] a = GetComponents<AudioSource>();
        {
            foreach(AudioSource s in a)
            {
                if(s.clip.name.StartsWith("RatWalk"))
                {
                    walk = s;
                }
                else if(s.clip.name.StartsWith("SmallRat") || s.clip.name.StartsWith("BigRat"))
                {
                    audio = s;
                }
            }
        }
    }

    void start()
    {
        //Get the navigation agent component
        agent = GetComponent<NavMeshAgent>();
        Invoke("startAgent", 0.5f);
        //Is movePositions set

        Player = GameObject.FindGameObjectWithTag("Player");
        ratSoundRandom();

    }

    void startAgent()
    {
        agent.enabled = true;


        if (MovePositions.Length > 0)
        {
            //Get the next movement position
            movePosition = GetNextPos();

            //Move to the position
            MoveTo(movePosition);
        }
        //find the player
    }

    void startedSet()
    {
        if (!Stats.IsDead)
        {
            MainTick = StartCoroutine(EnemyTick());
            MoveTo(SpawnPoint);
        }
    }

    bool started = false;
    bool enemyesDisabled = false;


    /// <summary>
    /// Sets the destination of the navigation
    /// </summary>
    /// <param name="Position">The destination</param>
    void MoveTo(Vector3 Position)
    {
        if (agent && agent.isActiveAndEnabled)
        {
            try
            {
                if (!isWalking)
                {
                    walkSound();
                }
                agent.destination = Position;
            }
            catch { }
        }
    }

    /// <summary>
    /// Tests if the enemy has reached its destination
    /// </summary>
    /// <returns></returns>
    bool TestPosition()
    {
        return transform.position.x == movePosition.x && transform.position.z == movePosition.z;
    }

    [SerializeField]
    public float spawnProtect = 5;

    /// <summary>
    /// Gets the Next position from the Move Positions array
    /// </summary>
    /// <returns></returns>
    Vector3 GetNextPos()
    {
        if (MovePositions.Length > 0)
        {
            //Get the next position
            Vector3 Target = MovePositions[CurTarget];

            //Change the thing
            if (CurTarget < MovePositions.Length - 1)
            {
                CurTarget++;
                return Target;
            }
            else
            {
                CurTarget = 0;
                return Target;
            }
        }
        return transform.position;
    }
    
    public void GetStats()
    {
        if(Motor.instance.stats.PlayerLevel == 1)
        {
            PlayerPrefs.DeleteKey(transform.name + "IsDead");
        }

        SpawnPoint = transform.position;
        DefaultStats.Position = transform.position;
        Stats.SpawnTime = 35;
        if (Vector3.Distance(Stats.Position, SpawnPoint) > 3 || Stats.Position != new Vector3(0,0,0))
        {
            transform.position = Stats.Position;
            Invoke("startedSet", spawnProtect);
        }
        else
        {
            transform.position = SpawnPoint;
            startedSet();
        }

        //Did this enemy come from a fight scene
        if (transform.name == PlayerPrefs.GetString("Enemy"))
        {
            Debug.Log.Add(name + " is Enemy");
            //Set health
            Stats.Health = PlayerPrefs.GetInt("EnemyHealth");

            //Does the enemy have 0 health
            if (Stats.Health <= 0)
            {
                //Kill the enemy
                Kill();
                if (Stats.DroppedItem != null && Stats.DroppedItem.Object != null)
                {
                    Invoke("itemSpawnDelay", 1);
                }
                //Give the player xp
                Motor.instance.stats.AddXp(Stats.xpReward);
                PlayerPrefs.DeleteKey("Enemy");
                PlayerPrefs.SetString("Enemy", "");
            }
        }
        else if (Stats.IsDead == true)
        {
            transform.Find("Graphics").gameObject.SetActive(false);
            transform.position = SpawnPoint;
            MoveTo(SpawnPoint);
            StartCoroutine(RespwawnTimer(Stats.ReamainingRespawnTime));
        }

        if(PlayerPrefs.HasKey(transform.name + "IsDead"))
        {
            if(PlayerPrefs.GetInt(transform.name + "IsDead") == 1)
            {
                Destroy(gameObject, 0.5f);
            }
        }
    }

    void itemSpawnDelay()
    {
        Instantiate(Stats.DroppedItem.Object, (GameManager.Control.curplayer.transform.position) + transform.forward * 3, Quaternion.identity);
    }

    public void SetStats()
    {
        Stats.Position = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

    /// <summary>
    /// Kills the enemy
    /// </summary>
    public void Kill()
    {
        //Set the enemy dead
        Stats.setDead(true);
        if (isQuestRat)
        {
            PlayerPrefs.SetInt(transform.name + "IsDead", 1);
            Destroy(gameObject, 1);
        }
        else
        {
            //Disable graphics
            transform.Find("Graphics").gameObject.SetActive(false);
            transform.position = SpawnPoint;
            MoveTo(SpawnPoint);
            if (Stats.ReamainingRespawnTime == Stats.SpawnTime)
            {
                StartCoroutine(RespwawnTimer(Stats.SpawnTime));
                Debug.Log.Add("1");
            }
            else
            {
                StartCoroutine(RespwawnTimer(Stats.ReamainingRespawnTime));
                Debug.Log.Add("2");
            }
        }
    }

    IEnumerator RespwawnTimer(float time)
    {
        Stats.ReamainingRespawnTime -= Time.deltaTime;
        yield return new WaitForSeconds(time);
        Respawn();
        Stats.ReamainingRespawnTime = Stats.SpawnTime;
    }

    void ratSoundRandom()
    {
        if(!Stats.IsDead)
        {
            audio.Play();
        }
        Invoke("ratSoundRandom", Random.Range(3, 15));      // Voi muuttaa
    }

    [SerializeField]
    float step = 0.5f;

    bool isWalking = false;
    void walkSound()
    {

        if (agent.velocity.magnitude == 0)
        {
            isWalking = false;
            CancelInvoke("walkSound");
            walk.Stop();
        }
        else
        {
            isWalking = true;
            walk.Play();
            Invoke("walkSound", step);
        }
    }

    void Respawn()
    {
        Stats.setDead(false);
        Stats.Health = DefaultStats.Health;
        transform.Find("Graphics").gameObject.SetActive(true);
        MainTick = StartCoroutine(EnemyTick());
    }

    bool isPlayed = false;

    IEnumerator EnemyTick()
    {
        while (true)
        {
            //is the player set
            if (Player != null && active)
            {
                //Is the enemy chasing the player and is the player dead
                if (Chasing && GameManager.Control.dead == false)
                {
                    //Is the enemy stil in range
                    if (Vector3.Distance(transform.position, Player.transform.position) < outerRadius + 2)
                    {
                        //Set the enemys destination to the players position
                        MoveTo(Player.transform.position);
                        if(isPlayed == false)
                        {
                            audio.Play();
                            isPlayed = true;
                        }

                        //Is the player close enough to start a fight
                        if (Vector3.Distance(transform.position, Player.transform.position) <= innerRadius)
                        {
                            //Save the game
                            SaveManager.instance.SaveGame();
                            PlayerPrefs.SetFloat("FightPosX", GameObject.FindGameObjectWithTag("Player").transform.position.x);
                            PlayerPrefs.SetFloat("FightPosY", GameObject.FindGameObjectWithTag("Player").transform.position.y);
                            PlayerPrefs.SetFloat("FightPosZ", GameObject.FindGameObjectWithTag("Player").transform.position.z);
                            PlayerPrefs.SetInt("EnemyDamage", Stats.Damage);
                            PlayerPrefs.SetInt("EnemyHealth", Stats.Health);
                            PlayerPrefs.SetInt("EnemyArmor", Stats.Armor);
                            PlayerPrefs.SetString("Enemy", transform.name);
                            PlayerPrefs.SetInt("EnemyType", Stats.Type);

                            //TestClient.Instance.sendMessage("Go fight");
                            Debug.Log.Add(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                            PlayerPrefs.SetString("SceneFight", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString());
                            SceneManager.instance.ChangeScene("FightScene");
                            //Kill();
                        }
                    }
                    else
                    {
                        //Stop chasing the player
                        isPlayed = false;
                        MoveTo(SpawnPoint);
                        Chasing = false;
                    }
                }
                else
                {
                    //Is the enemy near enough to the player to start chasing
                    if (Vector3.Distance(transform.position, Player.transform.position) < outerRadius)
                    {
                        Chasing = true;
                        Debug.Log.Add("Enemy Tick");
                    }
                }
                if (!Chasing)
                {
                    //Move to the things
                    if (TestPosition())
                    {
                        movePosition = GetNextPos();
                        MoveTo(movePosition);
                    }
                }
            }
            else
            {
                Player = GameObject.FindGameObjectWithTag("Player");
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    bool active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.StartsWith("City9"))
        {
            active = false;
            MoveTo(SpawnPoint);
            Invoke("returnActive", 5);
        }
    }

    void returnActive()
    {
        active = true;
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class EnemyStats
{
    public Vector3 Position;
    public bool IsDead;
    public int SpawnTime;
    public float ReamainingRespawnTime;

    public int Damage;
    public int Health;
    public int Armor;
    public int xpReward;

    public int Type;

    public Item DroppedItem;

    public void setDead(bool dead)
    {
        IsDead = dead;
    }
}