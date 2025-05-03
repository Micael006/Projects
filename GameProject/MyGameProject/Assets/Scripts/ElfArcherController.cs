using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfArcherController : MonoBehaviour
{
    public GameObject elfArcher;
    public GameObject player;
    public GameObject arrowPrefab;
    public float Speed { get; set; }
    public float maxHealth;
    public float curHealth;
    Animator anim;
    public int direction = 0;
    public bool isFighting = false;
    public bool isActing = false;
    public bool isBusy = false;
    public bool isDead = true;
    float angle;
    float aDist = 1f;
    public Coroutine control = null;
    void Start()
    {
        Speed = 1f * DataHolder.fixedFrameTime;
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        elfArcher = anim.gameObject;
        elfArcher.name = "elfArcher";
        anim.SetInteger("direction", direction);
        anim.SetBool("isFighting", isFighting);
    }

    public void StartActing()
    {
        Speed = 1f * DataHolder.fixedFrameTime;
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        elfArcher = anim.gameObject;
        anim.SetInteger("direction", direction);
        anim.SetBool("isFighting", isFighting);
        isActing = true;
        elfArcher.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void GotHurt()
    {
        curHealth -= player.GetComponent<PlayerControl>().damage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player.GetComponent<PlayerControl>().gameStopped)
        {
            angle = 180f / Mathf.PI * Mathf.Atan((player.transform.position.y - elfArcher.transform.position.y) / (player.transform.position.x - elfArcher.transform.position.x));
            angle = (player.transform.position.x < elfArcher.transform.position.x) ? 180 + angle : angle;
            if (isActing)
            {
                ElfArcherAction();
                if (control == null)
                {
                    control = StartCoroutine(Movement());
                }
            }
        }
    }

    private void ElfArcherAction()
    {
        float distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - elfArcher.transform.position.x, 2) + Mathf.Pow(player.transform.position.y - elfArcher.transform.position.y, 2));
        //Условие уничтожения (телепортация за карту)
        if (distance >= 25f)
        {
            isActing = false;
            StopCoroutine(control);
            control = null;
        }
        if(curHealth <= 0)
        {
            isDead = true;
            isActing = false;
            StopCoroutine(control);
            control = null;
        }
        else if (distance <= 15f && !isBusy && !player.GetComponent<PlayerControl>().gameStopped)
        {
            StartCoroutine(Fighting());
        }
    }

    IEnumerator Movement()
    {
        float xS, yS;
        while (true)
        {
            if (!player.GetComponent<PlayerControl>().gameStopped)
            {
                direction = (player.transform.position.x < elfArcher.transform.position.x) ? 1 : 0;
                anim.SetInteger("direction", direction);
                xS = Speed * Mathf.Cos(Mathf.PI * angle / 180f);
                yS = Speed * Mathf.Sin(Mathf.PI * angle / 180f);
                elfArcher.transform.position += new Vector3(xS, yS, 0);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator Fighting()
    {
        isBusy = true;
        //Сама атака
        yield return new WaitForSeconds(2f);
        isFighting = true;
        anim.SetBool("isFighting", isFighting);
        yield return new WaitForSeconds(0.5f);
        //Выстрел
        GameObject arrow = Instantiate(arrowPrefab, elfArcher.transform.position + new Vector3(aDist * Mathf.Cos(Mathf.PI * angle / 180f), aDist * Mathf.Sin(Mathf.PI * angle / 180f), 0), Quaternion.identity);
        arrow.transform.Rotate(0, 0, angle - 45f);
        yield return new WaitForSeconds(0.15f);
        isFighting = false;
        anim.SetBool("isFighting", isFighting);
        yield return new WaitForSeconds(1.35f);
        isBusy = false;
    }
}
