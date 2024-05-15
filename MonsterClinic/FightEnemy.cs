using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightEnemy : MonoBehaviour
{
    [SerializeField]
    int Health;

    int MaxHealth;
    Slider HealthSlider;
    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = Health;
        HealthSlider = transform.GetComponentInChildren<Slider>();
        HealthSlider.value = 1;
        floatPos = new Vector3(transform.position.x, 0.2f);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        HealthSlider.value = (float)Health / (float)MaxHealth;
        if (Health <= 0)
        {
            FightManager.instance.EndFight();
            Destroy(gameObject);
        }
        else StartCoroutine(TurnRed());


    }

    Vector3 floatPos;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, Time.deltaTime * 0.1f * (floatPos.y < 0 ? -1:1));
        if((floatPos.y > 0 && transform.position.y >= floatPos.y) || (floatPos.y < 0 && transform.position.y <= floatPos.y))
        {
            floatPos = new Vector3(floatPos.x, floatPos.y * -1);
            Debug.Log(floatPos);
        }
    }

    IEnumerator TurnRed()
    {
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.red;
        transform.GetChild(1).localScale = new Vector3(1.05f, 1.05f, 1.05f);
        GameObject g = GameObject.Find("HealthbarBase");
        yield return new WaitForSeconds(0.07f);
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(1).localScale = new Vector3(1, 1, 1);
    }
}
