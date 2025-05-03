using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public GameObject player;
    public GameObject bladePrefab;
    public GameObject GUI;
    public Camera camera;
    public bool gameStopped;
    public float Speed { get; set; }
    Animator anim;
    public int direction;
    public bool isMoving;
    public bool isFighting;
    bool alive;
    float bDist;
    float angle;
    Vector3 mousePos;
    public float border; //Границы по X, Y совпадают, так как карта - квадратная
    public int curHealth;
    public int maxHealth;
    public int damage;
    public int score;
    public int curXP;
    public int maxXP;
    public int level;

    void FixedUpdate()
    {
        if (alive && !gameStopped)
        {
            PlayerMovement();
        }
    }
    public void GetScore(int amount)
    {
        score += amount;
        curXP += amount;
        if(curXP >= maxXP)
        {
            curXP -= maxXP;
            level++;
            GUI.GetComponent<GUIController>().ShowBonusMenu();
            maxXP = (int)Mathf.Round(maxXP * 1.2f);
        }
        GUI.GetComponent<GUIController>().FixPlayerGUI();
    }
    public void StartGame()
    {
        gameStopped = false;
    }
    public void StopGame()
    {
        gameStopped = true;
    }

    public void ResetNewGame()
    {
        StopGame();
        direction = 0;
        isMoving = false;
        isFighting = false;
        alive = true;
        bDist = 1f;
        angle = 0f;
        mousePos = Vector3.zero;
        Speed = 4f * DataHolder.fixedFrameTime;
        border = (float)((DataHolder.mapSideLength - (DataHolder.freeRoamCoef + 1) * 2) * DataHolder.tileSize) / 2; //Границы по X, Y совпадают, так как карта - квадратная
        curHealth = 3;
        maxHealth = 3;
        damage = 40;
        score = 0;
        curXP = 0;
        maxXP = 10;
        level = 0;
        anim = GetComponent<Animator>();
        anim.SetInteger("direction", direction);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isFighting", isFighting);
        player.transform.position = Vector3.zero;
        GUI.GetComponent<GUIController>().FixPlayerGUI();
    }

    public void ResetLoadGame()
    {
        StopGame();
        direction = 0;
        isMoving = false;
        isFighting = false;
        alive = true;
        bDist = 1f;
        angle = 0f;
        mousePos = Vector3.zero;
        Speed = DataHolder.aDH.playerSpeed * DataHolder.fixedFrameTime;
        border = (float)((DataHolder.mapSideLength - (DataHolder.freeRoamCoef + 1) * 2) * DataHolder.tileSize) / 2; //Границы по X, Y совпадают, так как карта - квадратная
        curHealth = DataHolder.aDH.playerCurrentHealth;
        maxHealth = DataHolder.aDH.playerMaxHealth;
        damage = DataHolder.aDH.playerDamage;
        score = DataHolder.aDH.playerScore;
        curXP = DataHolder.aDH.playerCurrentXP;
        maxXP = DataHolder.aDH.playerMaxXP;
        level = DataHolder.aDH.playerLevel;
        anim = GetComponent<Animator>();
        anim.SetInteger("direction", direction);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isFighting", isFighting);
        player.transform.position = Vector3.zero;
        GUI.GetComponent<GUIController>().FixPlayerGUI();
    }

    public void GotHurt(string name)
    {
        curHealth = Mathf.Max(0, curHealth - 1);
        if(curHealth <= 0)
        {
            alive = false;
            direction = 0;
            isMoving = false;
            isFighting = false;
            anim.SetInteger("direction", direction);
            anim.SetBool("isMoving", isMoving);
            anim.SetBool("isFighting", isFighting);
            GUI.GetComponent<GUIController>().ShowLastPanel();
        }
        GUI.GetComponent<GUIController>().FixPlayerGUI();
    }

    GameObject lastCollision = null;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "arrow")
        {
            if (lastCollision != collision.gameObject)
            {
                lastCollision = collision.gameObject; 
                Destroy(collision.gameObject);
                GotHurt(collision.gameObject.name);
            }
        }
        else if (collision.gameObject.name == "elfArcher")
        {
            GotHurt(collision.gameObject.name);
        }
    }

    private void PlayerMovement()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !isFighting)
        {
            StartCoroutine(Fighting());
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                direction = 2;
                isMoving = true;
                if (transform.position.y < border)
                    transform.position += new Vector3(0, Speed, 0);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                direction = 1;
                isMoving = true;
                if (transform.position.x < border)
                    transform.position += new Vector3(Speed, 0, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                direction = 0;
                isMoving = true;
                if (transform.position.y > -border)
                    transform.position += new Vector3(0, -Speed, 0);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                direction = 3;
                isMoving = true;
                if (transform.position.x > -border)
                    transform.position += new Vector3(-Speed, 0, 0);
            }
            else
            {
                isMoving = false;
            }
        }
        anim.SetBool("isMoving", isMoving);
        anim.SetInteger("direction", direction);
        anim.SetBool("isFighting", isFighting);
    }

    IEnumerator Fighting()
    {
        isFighting = true;
        anim.SetBool("isFighting", isFighting);
        mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        angle = 180f / Mathf.PI * Mathf.Atan((mousePos.y - player.transform.position.y) / (mousePos.x - player.transform.position.x));
        angle = (mousePos.x < player.transform.position.x) ? 180 + angle : angle;
        if (angle < 45f && angle >= -45f)
        {
            direction = 1;
        }
        else if (angle < 135f && angle >= 45f)
        {
            direction = 2;
        }
        else if (angle < 225f && angle >= 135f)
        {
            direction = 3;
        }
        else
        {
            direction = 0;
        }
        anim.SetInteger("direction", direction);
        yield return new WaitForSeconds(0.35f);
        GameObject blade = Instantiate(bladePrefab, player.transform.position + new Vector3(bDist * Mathf.Cos(Mathf.PI * angle / 180f), bDist * Mathf.Sin(Mathf.PI * angle / 180f), 0), Quaternion.identity);
        blade.transform.Rotate(0, 0, angle);
        yield return new WaitForSeconds(0.25f);
        isFighting = false;
        anim.SetBool("isFighting", isFighting);
    }
}
