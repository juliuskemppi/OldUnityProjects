using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class GraphicsManager : MonoBehaviour
{

    [SerializeField, Header("Backgrounds")]
    GameObject WaitingRoom;

    [SerializeField]
    GameObject DoctorRoom;

    [SerializeField]
    GameObject BejeweledBackground;

    [SerializeField]
    GameObject FightBackground;

    [SerializeField, Header("UI")]
    GameObject CureButton;

    [SerializeField]
    GameObject Buttons;

    [SerializeField]
    GameObject FightUI;

    [SerializeField]
    GameObject BejeweledUI;

    [SerializeField]
    GameObject DayButton;

    [SerializeField]
    GameObject TransitionBlock;

    bool moving;

    void Update()
    {
        if(moving)
        {
            TransitionBlock.transform.position = Vector3.Lerp(TransitionBlock.transform.position, targetPos, Time.deltaTime * 5);
            if(Vector3.Distance(TransitionBlock.transform.position, targetPos) < 0.1f)
            {
                TransitionBlock.transform.position = targetPos;
                moving = false;
            }
        }
    }
    Vector2 targetPos;
    public void ShowWaitingRoom()
    {
        Buttons.SetActive(false);
        FightUI.SetActive(false);
        BejeweledUI.SetActive(false);
        DayButton.SetActive(false);
        CureButton.SetActive(false);

        moving = true;
        targetPos = new Vector3(-20, 0, 0);
        WaitingRoom.SetActive(true);
        //DoctorRoom.SetActive(false);
        FightBackground.SetActive(false);
        BejeweledBackground.SetActive(false);
    }

    public void ShowDoctorRoom(bool cured)
    {
        if(!cured) Buttons.SetActive(true);
        CureButton.SetActive(true);
        FightUI.SetActive(false);
        BejeweledUI.SetActive(false);

        moving = true;
        targetPos = new Vector3(0, 0, 0);

        //WaitingRoom.SetActive(false);
        DoctorRoom.SetActive(true);
        FightBackground.SetActive(false);
        BejeweledBackground.SetActive(false);
    }

    public void ShowFight()
    {
        Buttons.SetActive(false);
        FightUI.SetActive(true);
        CureButton.SetActive(false);

        WaitingRoom.SetActive(false);
        DoctorRoom.SetActive(false);
        FightBackground.SetActive(true);
        BejeweledBackground.SetActive(false);
    }

    public void ShowBejeweled()
    {
        Buttons.SetActive(false);
        BejeweledUI.SetActive(true);
        CureButton.SetActive(false);


        WaitingRoom.SetActive(false);
        DoctorRoom.SetActive(false);
        FightBackground.SetActive(false);
        BejeweledBackground.SetActive(true);
    }
}
