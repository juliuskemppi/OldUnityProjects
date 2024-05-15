using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649
public class Player : MonoBehaviour
{

    Patient CurPatient;
    GameObject Selected;
    [HideInInspector]
    public Bejeweled b;
    // Start is called before the first frame update
    void Start()
    {

    }

    Vector2 TouchStart;
    Vector2 TouchEnd;
    float HoldTime = 0.3f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin,ray.direction);
            if (b && b.ShowImage)
            {
                b.HideImage();
            }
            else if (hit.transform)
            {
                if (!GameManager.Instance.CureMode() && hit.transform.GetComponent<Patient>())
                {
                    if (CurPatient != null)
                    {
                        DeselectPatient();
                    }
                    SelectPatient(hit.transform.GetComponent<Patient>());
                }
                else if (hit.transform.GetComponent<GridObject>())
                {
                    TouchStart = Input.mousePosition;
                    BejeweledClick(hit.transform.gameObject);
                }
                else if (hit.transform.GetComponent<FightEnemy>())
                {
                    FightManager.instance.BacteriaAttack();
                }
            }
                    
        }
        if(Input.GetMouseButton(0))
        {
            HoldTime -= Time.deltaTime;
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(b && b.IsGameActive() && Selected != null && HoldTime <= 0)
            {
                HoldTime = 0.3f;
                BejeweledRelease();
            }
        }
        if(Input.GetKeyDown("space") && FightManager.instance && FightManager.instance.IsFighting())
        {
            FightManager.instance.BacteriaAttack();
        }
    }

    void Deselect()
    {
        Selected.GetComponent<GridObject>().Deselect();
        Selected = null;
    }

    public void SetPatient(Patient patient)
    {
        CurPatient = patient;
    }

    void SelectPatient(Patient patient)
    {
        SetPatient(patient);
        patient.transform.SetParent(null);
        GameManager.Instance.SelectPatient(patient);
    }

    void DeselectPatient()
    {
        GameManager.Instance.DeselectPatient();
    }

    void BejeweledClick(GameObject block)
    {
        if (b.Falling())
        {
            if (!Selected)
            {
                Selected = block;
                Selected.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                if (Selected == block)
                {
                    Deselect();
                }
                else if (b.IsAdjacent(Selected, block))
                {
                    b.Swapper(Selected, block);
                    block.GetComponent<GridObject>().Done = false;
                    Deselect();
                }
                else
                {
                    Deselect();
                    Selected = block;
                    Selected.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }
    void BejeweledRelease()
    {

        TouchEnd = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        Vector2 TouchDif = TouchEnd - TouchStart;
        Debug.Log(TouchDif);
        bool CanSwap = false;
        if((Mathf.Abs(TouchDif.x) / 2 > Mathf.Abs(TouchDif.y) || Mathf.Abs(TouchDif.y) / 2 > Mathf.Abs(TouchDif.x)) && TouchDif.magnitude > 40)
        {
            CanSwap = true;
        }
        if (hit.transform && hit.transform.GetComponent<GridObject>())
        {
            GameObject block = hit.transform.gameObject;
            if (b.IsAdjacent(Selected, block))
            {
                b.Swapper(Selected, block);
                block.GetComponent<GridObject>().Done = false;
                Deselect();
            }
            else if(CanSwap)
            {
                ReleaseSwap();
            }
        }
        else if (CanSwap)
        {
            ReleaseSwap();
        }
    }

    void ReleaseSwap()
    {
        Vector2 dif = TouchEnd - TouchStart;
        Vector2 Loc = b.FindObject(Selected);
        if(dif.x > 0 && dif.x > Mathf.Abs(dif.y))
        {
            b.Swap(Loc,  Loc + new Vector2(1, 0));
        }
        else if (dif.x < 0 && Mathf.Abs(dif.x) > Mathf.Abs(dif.y))
        {
            b.Swap(Loc, Loc + new Vector2(-1, 0));
        }
        else if (dif.y > 0 && dif.y > Mathf.Abs(dif.x))
        {
            b.Swap(Loc, Loc + new Vector2(0, 1));
        }
        else if (dif.y < 0 && Mathf.Abs(dif.x) < Mathf.Abs(dif.y))
        {
            b.Swap(Loc, Loc + new Vector2(0, -1));
        }

    }
}
