using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

public class FightManager : MonoBehaviour
{
    bool PlayerTurn;
    int playerHealth;
    int EnemyHealth;

    bool Fighting;

    [SerializeField]
    GameObject PlayerHealthBar;

    [SerializeField]
    GameObject EnemyHealthBar;

    [SerializeField]
    GameObject EnemyType;

    int pStartHealth;
    int eStartHealth;

    [SerializeField]
    GameObject AttackButton;

    DiseaseType enemyDisease;

    public static FightManager instance;

    FightEnemy Enemy;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerTurn = true;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFight(int PlayerHP, int EnemyHP)
    {
        playerHealth = PlayerHP;
        pStartHealth = PlayerHP;
        EnemyHealth = EnemyHP;
        eStartHealth = EnemyHP;
        SpawnEnemy();
        Fighting = true;
        AttackButton.SetActive(true);
        EnemyHealthBar.GetComponent<Slider>().value = 1;
        PlayerHealthBar.GetComponent<Slider>().value = 1;
        enemyDisease = GameManager.Instance.CurPatient.GetDisease();
    }

    void SetTurn()
    {
        PlayerTurn = !PlayerTurn;
        if(PlayerTurn)
        {
            AttackButton.SetActive(true);
        }
        else
        {
            StartCoroutine(EnemyTurn());
            AttackButton.SetActive(false);
        }
    }

    public void Attack(DiseaseType type)
    {
        if (PlayerTurn)
        {
            if (type == enemyDisease)
                Enemy.TakeDamage(7);
            else Enemy.TakeDamage(2);
            //EnemyHealthBar.GetComponent<Slider>().value = (float)EnemyHealth / (float)eStartHealth;
        }
        else
        {
            playerHealth -= 5;
            PlayerHealthBar.GetComponent<Slider>().value = (float)playerHealth / (float)pStartHealth;
        }
        if (EnemyHealth <= 0)
        {
            EndFight();
        }
        else SetTurn();
    }

    public void EndFight()
    {
        Fighting = false;
        GameManager.Instance.CurPatient.gameObject.SetActive(true);
        GameManager.Instance.CurPatient.Cure();
        GameManager.Instance.ShowReturn();
        AttackButton.SetActive(false);
    }

    public void BacteriaAttack()
    {
        Attack(DiseaseType.Bacteria);
    }

    public void VirusAttack()
    {
        Attack(DiseaseType.Virus);
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.5f);
        Attack(enemyDisease);
    }

    public bool IsFighting()
    {
        return Fighting;
    }

    void SpawnEnemy()
    {
       Enemy = Instantiate(EnemyType, new Vector3(5, 0), Quaternion.identity).GetComponent<FightEnemy>();
    }
}
