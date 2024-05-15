using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [HideInInspector]
    public Color color;

    [HideInInspector]
    public bool Done;

    bool Falling = false;

    bool Swapping;

    [HideInInspector]
    public bool Fallen = false;
    Vector3 targetPos = new Vector3(0, 0, 5);

    [HideInInspector]
    public int Type;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetColor(Color c)
    {
        color = c;
    }

    // Update is called once per frame
    void Update()
    {
        if (Falling && targetPos.z <= 0)
        {
            transform.Translate(new Vector3(0, -5 * Time.deltaTime));
            if (Mathf.Abs(transform.position.y - targetPos.y) < .045 || transform.position.y < targetPos.y)
            {
                transform.position = targetPos;
                Falling = false;
                Fallen = true;
            }
        }
        else if (Swapping)
        {
            Vector3 Dir = Vector3.Normalize(transform.position - targetPos);
            transform.Translate(-Dir * 6 * Time.deltaTime);
            if(Vector3.Distance(transform.position,targetPos) <.05f)
            {
                transform.position = targetPos;
                Swapping = false;
                FindObjectOfType<Bejeweled>().EndSwap();
            }
        }
    }

    public void Deselect()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        Done = false;
    }

    public void SetDone(bool done)
    {
        Done = done;
    }

    public void StartFall(Vector3 Target)
    {
        targetPos = Target;
        Falling = true;
    }

    public bool IsFalling()
    {
        return Falling;
    }

    public void StartSwap(Vector3 target)
    {
        targetPos = target;
        Swapping = true;
    }
}
