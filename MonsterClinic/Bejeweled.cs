using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

public class Bejeweled : MonoBehaviour
{
    [SerializeField, Header("Objects")]
    GameObject ob;

    [SerializeField]
    List<Sprite> Sprites = new List<Sprite>();

    GameObject[,] grid;

    int WantedType;

    [SerializeField]
    int GridWidth;

    [SerializeField]
    int GridHeight;

    [SerializeField]
    GameObject TrailObject;

    [SerializeField]
    int TargetAmount;

    [SerializeField, Header("UI Elements")]
    Text text;

    [SerializeField]
    Image DisplayImage;

    [SerializeField]
    Image DisplayImageSmall;

    [HideInInspector]
    public bool ShowImage;

    bool Active;

    // Start is called before the first frame update
    void Start()
    {
    }

    int Points;

    // Update is called once per frame<
    void Update()
    {

    }

    public void StartGame()
    {
        Active = true;
        CreateGrid(GridWidth, GridHeight);
        Points = 0;
        text.text = "0/" + TargetAmount.ToString();
        WantedType = Random.Range(0, Sprites.Count);
        DisplayImage.transform.parent.gameObject.SetActive(true);
        DisplayImage.sprite = Sprites[WantedType];
        DisplayImageSmall.sprite = Sprites[WantedType];
        Invoke("Show", 0.1f);
    }

    void Show()
    {
        ShowImage = true;
    }

    public void HideImage()
    {
        ShowImage = false;
        DisplayImage.transform.parent.gameObject.SetActive(false);
    }
    void CreateGrid(int x, int y)
    {
        grid = new GameObject[x, y];
        float xStart = transform.position.x - grid.GetLength(0) / 2;
        float yStart = transform.position.y - grid.GetLength(1) / 2;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int a = 0; a < grid.GetLength(1); a++)
            {
                GameObject obect = Instantiate(ob, new Vector3(xStart + i, yStart + a, 0), Quaternion.identity);
                int q = Random.Range(0, Sprites.Count);
                obect.GetComponent<SpriteRenderer>().sprite = Sprites[q];
                obect.GetComponent<GridObject>().SetColor(obect.GetComponent<SpriteRenderer>().color);
                obect.GetComponent<GridObject>().Type = q;
                grid[i, a] = obect;
            }
        }
    }

    public Vector2 FindObject(GameObject obj)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int a = 0; a < grid.GetLength(1); a++)
            {
                if (obj == grid[i, a])
                {
                    return new Vector2(i, a);
                }
            }
        }
        return new Vector2(-1, -1);
    }

    List<Vector2> FindNext(Vector2 pos, Color col, Vector2 Last)
    {
        List<Vector2> hi = new List<Vector2>();
        if (GetObject(pos) && !GetObject(pos).GetComponent<GridObject>().Done)
        {
            GetObject(pos).GetComponent<GridObject>().Done = true;
            hi.Add(pos);
        }
        else return hi;
        if (Last.x != -1 && pos.x + 1 < grid.GetLength(0))
            if (GetObject((int)pos.x + 1, (int)pos.y) && GetObject((int)pos.x + 1, (int)pos.y).GetComponent<GridObject>().color == col)
                hi.AddRange(FindNext(new Vector2(pos.x + 1, pos.y), col, new Vector2(1, 0)));
        if (Last.x != 1 && pos.x - 1 >= 0)
            if (GetObject((int)pos.x - 1, (int)pos.y) && GetObject((int)pos.x - 1, (int)pos.y).GetComponent<GridObject>().color == col)
                hi.AddRange(FindNext(new Vector2(pos.x - 1, pos.y), col, new Vector2(-1, 0)));
        if (Last.y != -1 && pos.y + 1 < grid.GetLength(1) && GetObject((int)pos.x, (int)pos.y + 1) && GetObject((int)pos.x, (int)pos.y + 1).GetComponent<GridObject>().color == col)
            hi.AddRange(FindNext(new Vector2(pos.x, pos.y + 1), col, new Vector2(0, 1)));
        if (Last.y != 1 && pos.y - 1 >= 0 && GetObject((int)pos.x, (int)pos.y - 1) && GetObject((int)pos.x, (int)pos.y - 1).GetComponent<GridObject>().color == col)
            hi.AddRange(FindNext(new Vector2(pos.x, pos.y - 1), col, new Vector2(0, -1)));
        return hi;
    }

    List<Vector2> FindNext(Vector2 pos, int type, Vector2 Last)
    {
        List<Vector2> hi = new List<Vector2>();
        if (GetObject(pos) && !GetObject(pos).GetComponent<GridObject>().Done)
        {
            GetObject(pos).GetComponent<GridObject>().Done = true;
            hi.Add(pos);
        }
        else return hi;
        if (Last.x != -1 && pos.x + 1 < grid.GetLength(0))
            if (GetObject((int)pos.x + 1, (int)pos.y) && GetObjectType(new Vector2(pos.x + 1, pos.y)) == type)
                hi.AddRange(FindNext(new Vector2(pos.x + 1, pos.y), type, new Vector2(1, 0)));
        if (Last.x != 1 && pos.x - 1 >= 0)
            if (GetObject((int)pos.x - 1, (int)pos.y) && GetObjectType(new Vector2(pos.x - 1, (int)pos.y))== type)
                hi.AddRange(FindNext(new Vector2(pos.x - 1, pos.y), type, new Vector2(-1, 0)));
        if (Last.y != -1 && pos.y + 1 < grid.GetLength(1) && GetObject((int)pos.x, (int)pos.y + 1) && GetObjectType(new Vector2(pos.x, pos.y + 1))== type)
            hi.AddRange(FindNext(new Vector2(pos.x, pos.y + 1), type, new Vector2(0, 1)));
        if (Last.y != 1 && pos.y - 1 >= 0 && GetObject((int)pos.x, (int)pos.y - 1) && GetObjectType(new Vector2(pos.x, pos.y - 1)) == type)
            hi.AddRange(FindNext(new Vector2(pos.x, pos.y - 1), type, new Vector2(0, -1)));
        return hi;
    }

    public bool IsAdjacent(GameObject ob1, GameObject ob2)
    {
        var vec1 = FindObject(ob1);
        var vec2 = FindObject(ob2);
        if ((Mathf.Abs(vec2.x - vec1.x) == 1 && vec2.y - vec1.y == 0) || (Mathf.Abs(vec2.y - vec1.y) == 1 && vec2.x - vec1.x == 0))
        {
            return true;
        }
        return false;
    }

    public void Swap(Vector2 one, Vector2 two)
    {
        SwapPosOne = two;
        SwapPosTwo = one;
        GameObject a = GetObject(one);
        grid[(int)one.x, (int)one.y] = GetObject(two);
        GetObject(one).GetComponent<GridObject>().StartSwap(GetWorldPos(one));
        grid[(int)two.x, (int)two.y] = a;
        GetObject(two).GetComponent<GridObject>().StartSwap(GetWorldPos(two));
    }
    public void Swapper(GameObject a, GameObject b)
    {
        if (a.GetComponent<GridObject>().Type != b.GetComponent<GridObject>().Type)
        {
            Vector2 aPos = FindObject(a);
            Vector2 bPos = FindObject(b);
            Swap(aPos, bPos);
        }
    }
    
    bool Swapping;
    Vector2 SwapPosOne;
    Vector2 SwapPosTwo;

    bool Ending;
    public void EndSwap()
    {
        if (Swapping)
        {
            if (!Ending)
                StartCoroutine(Continue(SwapPosOne, SwapPosTwo));
            else Ending = false;
            Swapping = false;
        }
        else Swapping = true;

    }

    IEnumerator Continue(Vector2 newa, Vector2 newb)
    {
        List<Vector2> RightBlocks = new List<Vector2>();
        yield return new WaitForSeconds(0.05f);
        int WantedAmount = 0;
        List<Vector2> aThings = new List<Vector2>();
        List<Vector2> bThings = new List<Vector2>();
        aThings.AddRange(FindNext(newa, GetObjectType(newa), new Vector2(0, 0)));
        bThings.AddRange(FindNext(newb, GetObjectType(newb), new Vector2(0, 0)));
       
        bool aT = aThings.Count > 2;
        bool bT = bThings.Count > 2;

        if (aT && GetObjectType(newa) == WantedType)
        {
            WantedAmount = aThings.Count;
            RightBlocks.AddRange(aThings);
        }
        else if (bT && GetObjectType(newb) == WantedType)
        {
            WantedAmount = bThings.Count;
            RightBlocks.AddRange(bThings);
        }
        List<Vector2> Things = new List<Vector2>();
        if (aT)
        {
            Things.AddRange(aThings);
        }
        if (bT)
        {
            Things.AddRange(bThings);
        }
        if (Things.Count > 2)
        {
            HighlightBlocks(Things);
            yield return new WaitForSeconds(0.05f);
            RemoveBlocks(Things);
            ResetBlocks(Things);
            yield return new WaitForSeconds(0.1f);
            if (RightBlocks.Count > 2)
            {
                foreach (Vector2 item in RightBlocks)
                {
                    Instantiate(TrailObject, GetWorldPos(new Vector3(item.x, item.y)), Quaternion.identity);
                }
            }
            yield return new WaitForSeconds(0.05f);
            Refill();
        }
        else
        {
            Ending = true;
            Swap(newa, newb);
        }
        Invoke("ResetThings", 0.1f);
    }

    void ResetThings()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int a = 0; a < grid.GetLength(1); a++)
            {
                if (GetObject(i, a) != null)
                    GetObject(i, a).GetComponent<GridObject>().SetDone(false);
            }
        }
        StartCoroutine(TestFallen());
    }

    GameObject GetObject(Vector2 pos)
    {
        return grid[(int)pos.x, (int)pos.y];
    }

    GameObject GetObject(int x, int y)
    {
        return grid[x, y];
    }

    Vector3 GetWorldPos(Vector2 GridPos)
    {
        Vector3 pos;
        float xStart = transform.position.x - grid.GetLength(0) / 2;
        float yStart = transform.position.y - grid.GetLength(1) / 2;
        pos = new Vector3(xStart + GridPos.x, yStart + GridPos.y);
        return pos;
    }

    Vector3 GetWorldPos(int x, int y)
    {
        Vector3 pos;
        float xStart = transform.position.x - grid.GetLength(0) / 2;
        float yStart = transform.position.y - grid.GetLength(1) / 2;
        pos = new Vector3(xStart + x, yStart + y);
        return pos;
    }

    void ResetLine(Vector2 Start)
    {
        for (int i = (int)Start.y; i < grid.GetLength(1); i++)
        {
            if (GetObject((int)Start.x, i) == null)
            {
                GameObject ob;
                if (GetFirstAbove((int)Start.x, i, out ob))
                {
                    grid[(int)Start.x, i] = ob;
                    ob.GetComponent<GridObject>().StartFall(GetWorldPos(new Vector2(Start.x, i)));
                }
            }
        }
    }

    bool GetFirstAbove(int x, int y, out GameObject FirstObject)
    {
        for (int i = y; i < grid.GetLength(1); i++)
        {
            var a = GetObject(x, i);
            if (a != null)
            {
                FirstObject = a;
                grid[x, i] = null;
                return true;
            }
        }
        FirstObject = null;
        return false;
    }

    void Refill()
    {
        if (Active)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int a = 0; a < grid.GetLength(1); a++)
                {
                    if (GetObject(i, a) == null)
                    {
                        GameObject obect = Instantiate(ob, new Vector3(GetWorldPos(i, a).x, 2 + a), Quaternion.identity);
                        int c = Random.Range(0, Sprites.Count);
                        obect.GetComponent<SpriteRenderer>().sprite = Sprites[c];
                        obect.GetComponent<GridObject>().Type = c;
                        //obect.GetComponent<GridObject>().SetColor(obect.GetComponent<SpriteRenderer>().color);
                        grid[i, a] = obect;
                        obect.GetComponent<GridObject>().StartFall(GetWorldPos(i, a));
                    }
                }
            }
        }
    }

    public void AddPoints(int points)
    {
        Points += points;
        text.text = Points.ToString() + "/" + TargetAmount.ToString();
        if(Points >= TargetAmount)
        {
            GameManager.Instance.CurPatient.gameObject.SetActive(true);
            GameManager.Instance.CurPatient.Cure();
            GameManager.Instance.ShowReturn();
            RemoveGrid();
        }
    }

    public bool Falling()
    {
            for (int a = 0; a < grid.GetLength(0); a++)
            {
                if (!GetObject(a, grid.GetLength(1)-1))
                {
                    return false;
                }
                bool c = GetObject(a, grid.GetLength(1)-1).GetComponent<GridObject>().IsFalling();
                if (c)
                {
                    return false;
                }
        }
        return true;
    }

    IEnumerator TestFallen()
    {
        while (!Falling())
        {
            yield return new WaitForSeconds(0.1f);
        }
        List<Vector2> Blocks = new List<Vector2>();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int a = 0; a < grid.GetLength(1); a++)
            {
                if (GetObject(i, a) != null && GetObject(i,a).GetComponent<GridObject>().Fallen)
                {
                    Blocks.Add(new Vector2(i, a));
                    GetObject(i, a).GetComponent<GridObject>().Fallen = false;
                }
            }
        }
        if(Blocks.Count > 0)
        StartCoroutine(HandleBlocks(Blocks));
    }

    IEnumerator HandleBlocks(List<Vector2> blocks)
    {
        List<Vector2> AllThings = new List<Vector2>();
        List<Vector2> RightBlocks = new List<Vector2>();
        int WantedAmount = 0;
        foreach (Vector2 pos in blocks)
        {
            if (GetObject(pos))
            {
                List<Vector2> Thing = FindNext(pos, GetObjectType(pos), new Vector2(0, 0));
                if (Thing.Count > 2)
                {
                    if(GetObjectType(pos) == WantedType)
                    {
                        WantedAmount += Thing.Count;
                        RightBlocks.AddRange(Thing);
                    }
                    AllThings.AddRange(Thing);
                }
            }
        }
        if (AllThings.Count > 2)
        {
            HighlightBlocks(AllThings);
            yield return new WaitForSeconds(0.05f);
            RemoveBlocks(AllThings);
            ResetBlocks(AllThings);
            yield return new WaitForSeconds(0.07f);
            //AddPoints(WantedAmount);
            if (RightBlocks.Count > 2)
            {
                foreach (Vector2 item in RightBlocks)
                {
                    Instantiate(TrailObject, GetWorldPos(new Vector3(item.x, item.y)), Quaternion.identity);
                }
            }
            yield return new WaitForSeconds(0.05f);
            Refill();
        }
        Invoke("ResetThings", 0.1f);
    }

    void RemoveGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int a = 0; a < grid.GetLength(1); a++)
            {
                Destroy(GetObject(i, a));
            }
        }
        text.text = "0/20";
        Points = 0;
        Active = false;
    }

    void HighlightBlocks(List<Vector2> blocks)
    {
        foreach (Vector2 item in blocks)
        {
            GetObject(item).GetComponent<SpriteRenderer>().color = Color.green;
            GetObject(item).GetComponent<GridObject>().Done = false;
        }
    }

    void RemoveBlocks(List<Vector2> blocks)
    {
        foreach (Vector2 item in blocks)
        {
            Destroy(GetObject(item));
            grid[(int)item.x, (int)item.y] = null;
        }
    }

    void ResetBlocks(List<Vector2> blocks)
    {
        foreach (Vector2 item in blocks)
        {
            ResetLine(item);
        }
    }

    int GetObjectType(Vector2 Pos)
    {
        return GetObject(Pos).GetComponent<GridObject>().Type;
    }

    public bool IsGameActive()
    {
        return Active;
    }
}
