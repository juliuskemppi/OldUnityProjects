using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable 649

public struct Day
{
    public int MonsterAmount;
    public int TimeAmount;
    
    public Day(int time, int monsters)
    {
        TimeAmount = time;
        MonsterAmount = monsters;
    }
}

public class GameManager : MonoBehaviour
{

    [SerializeField, Header("Patients")]
    GameObject patient;

    [SerializeField]
    List<GameObject> Patients = new List<GameObject>();

    [SerializeField]
    int patientAmount;

    [SerializeField]
    GameObject Bejeweled;

    [SerializeField]
    int TimeRemaining;

    public Text TimerText;
    public Text DayText;
    public Text MonstersCuredText;

    List<Day> days = new List<Day>() { new Day(300, 5), new Day(310, 7), new Day(350, 10), new Day(400, 15), new Day(400, 20), new Day(350, 25), new Day (350, 30), new Day (350, 35) };

    [SerializeField]
    GameObject CureButton;

    [HideInInspector]
    public Patient CurPatient;

    Player player;

    [SerializeField]
    GameObject DayButton;

    [SerializeField]
    GraphicsManager graphicsManager;


    public static GameManager Instance;

    [SerializeField, Header("Misc.")]
    Bejeweled bejeweled;

    GameObject PatientHolder;

    int CuredPatientAmount;

    int CurDay;

    int Monsters;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple Game Managers in scene");
        }
    }

    List<GameObject> PatientSpawns = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        PatientHolder = GameObject.Find("PatientHolder");
        TimerText = GameObject.Find("TimeText").GetComponent<Text>();
        TimerText.text = "Time: " + days[CurDay].TimeAmount;
        Monsters = days[CurDay].MonsterAmount;
        StartCoroutine(Timer());
        GameObject.Find("CuredText").GetComponent<Text>().text = "Cured Patients: 0" + "/" + Monsters;
        var patientSpawns = FindObjectsOfType<PatientSpawn>();

        foreach (PatientSpawn spawn in patientSpawns)
        {
            PatientSpawns.Add(spawn.gameObject);
        }

        for (int i = 0; i < patientAmount; i++)
        {
            SpawnPatient();
        }

    }

    [HideInInspector]
    public List<Patient> patients = new List<Patient>();

    //Gives the player a random minigame to play.
    public void RandomMinigame()
    {
        int minigameindex = Random.Range(1, 3);

        if (minigameindex == 1)
        {
           SpawnBejeweled();
            Debug.Log(minigameindex);
        }

        if(minigameindex == 2)
        {
           StartFight();
            Debug.Log(minigameindex);
        }
    }

    void SpawnPatient()
    {
        graphicsManager.ShowWaitingRoom();
        Patient a = Instantiate(Patients[Random.Range(0, Patients.Count)], PatientHolder.transform).GetComponent<Patient>();
        patients.Add(a);
        int b = patients.IndexOf(a);
        for (int i = 0; i < patients.Count; i++)
        {
            patients[i].transform.position = PatientSpawns[i].transform.position;
        }
        ShowUI();
    }

    public void SpawnBejeweled()
    {
        graphicsManager.ShowBejeweled();
        player.b = bejeweled;
        bejeweled.StartGame();
        CurPatient.gameObject.SetActive(false);
        PatientHolder.SetActive(false);
    }

    public void StartFight()
    {
        graphicsManager.ShowFight();
        FightManager.instance.SetFight(20, 20);
        CurPatient.gameObject.SetActive(false);
        PatientHolder.SetActive(false);

    }

    public void ShowUI()
    {
        PatientHolder.SetActive(true);
        graphicsManager.ShowWaitingRoom();
    }

    public void ReturnToWaitingRoom()
    {
        graphicsManager.ShowWaitingRoom();
        if (CurPatient.IsCured())
        {
            NewPatient();
        }
        else
        {

            DeselectPatient();
        }
    }

    public void NewPatient()
    {
        patients.RemoveAt(patients.IndexOf(CurPatient));
        CuredPatientAmount++;
        GameObject.Find("CuredText").GetComponent<Text>().text = "Cured Patients: " + CuredPatientAmount+ "/" + Monsters;
        Destroy(CurPatient.gameObject);
        if (CuredPatientAmount >= Monsters)
        {
            ShowNewDay();
        }
        else Invoke("SpawnPatient",0.1f);
    }

    public void ShowReturn()
    {
        graphicsManager.ShowDoctorRoom(true);
    }

    public void SelectPatient(Patient patient)
    {
        graphicsManager.ShowDoctorRoom(false);
        PatientHolder.SetActive(false);
        patient.transform.position = new Vector3(0, -1.1f, 0);
        patient.GetComponent<SpriteRenderer>().sortingOrder = 1;
        CurPatient = patient;
        patient.ToggleSpeechBubble(true);
    }



    void ShowPatientUI()
    {
        GameObject.Find("SpeechBubble").SetActive(true);
    }

    public bool CureMode()
    {
        return CureButton.activeSelf;
    }

    public void DeselectPatient()
    {
        PatientHolder.SetActive(true);
        CurPatient.transform.parent = PatientHolder.transform;
        CurPatient.GetComponent<SpriteRenderer>().sortingOrder = 0;
        CurPatient.ToggleSpeechBubble(false);

        for (int i = 0; i < patients.Count; i++)
        {
            patients[i].transform.position = PatientSpawns[i].transform.position;
        }
    }

    IEnumerator Timer()
    {
        while (TimeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            TimeRemaining--;
            TimerText.text = "Time: " + TimeRemaining;
            if(TimeRemaining <= 0)
            {
                //Loading The Game Over-Screen after time is out
                SceneManager.LoadScene(4);
                Debug.Log("Time Out");

            }
        }
    }

    void ShowNewDay()
    {
        DayButton.SetActive(true);
    }

    public void NewDay()
    {
        if (CurDay < days.Count)
        {
            Debug.Log("New Day");
            CurDay++;
            TimeRemaining = days[CurDay].TimeAmount;
            Monsters = days[CurDay].MonsterAmount;
            CuredPatientAmount = 0;
            GameObject.Find("CuredText").GetComponent<Text>().text = "Cured Patients: 0" + "/" + Monsters;
        }
        Invoke("SpawnPatient", 0.01f);
        //Changing the day text according to the amount of monsters you have to cure during each day.
        if(Monsters == 5)
        {
            DayText.text = "Day 1";
        }

        if(Monsters == 7)
        {
            DayText.text = "Day 2";
        }

        if(Monsters == 10)
        {
            DayText.text = "Day 3";
        }

        if(Monsters == 15)
        {
            DayText.text = "Day 4";
        }

        if(Monsters == 20)
        {
            DayText.text = "day 5";
        }

        if (Monsters == 25)
        {
            DayText.text = "Day 6";
        }

        if(Monsters == 30)
        {
            DayText.text = "Day 7";
        }

        //When the player has finished all 7 days they win the game.
        if (Monsters == 35)
        {
            //Hiding the UI-texts and loading Win Screen
            TimerText.gameObject.SetActive(false);
            DayText.gameObject.SetActive(false);
            MonstersCuredText.gameObject.SetActive(false);
            SceneManager.LoadScene(3);
        }
    }
}
