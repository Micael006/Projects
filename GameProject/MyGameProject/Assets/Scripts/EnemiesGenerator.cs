using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesGenerator : MonoBehaviour
{
    GameObject player;
    Transform generator;
    public Coroutine control;
    float r;
    public int firstNullIndex;
    static int poolSize;
    public bool isBusy;
    public GameObject elfArcherPrefab;
    GameObject[] activeEnemies;
    GameObject[] pool;
    System.Random angleChooser;
    float angle;
    public float startHealth;

    void FixedUpdate()
    {
        if (generator != null)
        {
            if (!generator.parent.gameObject.GetComponent<PlayerControl>().gameStopped)
            {
                firstNullIndex = activeEnemies.Length;
                //Проверка активных врагов на смерть
                for (int i = 0; i < activeEnemies.Length; i++)
                {
                    if (activeEnemies[i] != null)
                    {
                        if (activeEnemies[i].GetComponent<ElfArcherController>().isActing == false)
                        {
                            if (activeEnemies[i].GetComponent<ElfArcherController>().isDead)
                            {
                                int scoreAmount = (int)(activeEnemies[i].GetComponent<ElfArcherController>().maxHealth / 50);
                                player.GetComponent<PlayerControl>().GetScore(scoreAmount);
                            }
                            activeEnemies[i].transform.parent = generator;
                            activeEnemies[i].transform.localPosition = new Vector3(-40, 0, -1);
                            activeEnemies[i].GetComponent<SpriteRenderer>().sortingOrder = -1;
                            activeEnemies[i] = null;
                        }
                    }
                    else
                    {
                        firstNullIndex = Mathf.Min(i, firstNullIndex);
                    }
                }
                if (firstNullIndex < activeEnemies.Length && !isBusy)
                {
                    StartCoroutine(SpawnEnemy());
                }
            }
        }
    }

    public void ResetNewGame()
    {
        if(control != null)
        {
            StopCoroutine(control);
        }
        control = null;
        r = 15f;
        firstNullIndex = 0;
        poolSize = 10;
        isBusy = false;
        if(pool != null)
        {
            for(int i = 0; i < pool.Length; i++)
            {
                Destroy(pool[i]);
            }
        }
        activeEnemies = new GameObject[poolSize];
        pool = new GameObject[poolSize];
        angleChooser = new System.Random();
        startHealth = 50;
        player = GameObject.Find("Player");
        generator = GetComponent<Transform>();
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(elfArcherPrefab);
            pool[i].transform.parent = generator;
            pool[i].transform.localPosition = new Vector3(-40, 0, -1);
            pool[i].GetComponent<SpriteRenderer>().sortingOrder = -1;
            pool[i].GetComponent<ElfArcherController>().isActing = false;
            activeEnemies[i] = null;
        }
        control = StartCoroutine(IncreaseHealth());
    }

    public void ResetLoadGame()
    {
        if (control != null)
        {
            StopCoroutine(control);
        }
        control = null;
        r = 15f;
        firstNullIndex = 0;
        poolSize = 10;
        isBusy = false;
        if (pool != null)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                Destroy(pool[i]);
            }
        }
        activeEnemies = new GameObject[poolSize];
        pool = new GameObject[poolSize];
        angleChooser = new System.Random();
        startHealth = DataHolder.aDH.enemiesHealth;
        player = GameObject.Find("Player");
        generator = GetComponent<Transform>();
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(elfArcherPrefab);
            pool[i].transform.parent = generator;
            pool[i].transform.localPosition = new Vector3(-40, 0, -1);
            pool[i].GetComponent<SpriteRenderer>().sortingOrder = -1;
            pool[i].GetComponent<ElfArcherController>().isActing = false;
            activeEnemies[i] = null;
        }
        control = StartCoroutine(IncreaseHealth());
    }

    IEnumerator SpawnEnemy()
    {
        isBusy = true;
        yield return new WaitForSeconds(0.5f);
        activeEnemies[firstNullIndex] = pool[firstNullIndex];
        angle = Mathf.PI / 180f * angleChooser.Next(360);
        activeEnemies[firstNullIndex].transform.localPosition = new Vector3(r * Mathf.Cos(angle), r * Mathf.Sin(angle), 0);
        activeEnemies[firstNullIndex].transform.parent = null;
        activeEnemies[firstNullIndex].GetComponent<ElfArcherController>().maxHealth = startHealth;
        activeEnemies[firstNullIndex].GetComponent<ElfArcherController>().curHealth = startHealth;
        activeEnemies[firstNullIndex].GetComponent<ElfArcherController>().isDead = false;
        activeEnemies[firstNullIndex].GetComponent<ElfArcherController>().StartActing();
        yield return new WaitForSeconds(1.5f);
        isBusy = false;
    }

    IEnumerator IncreaseHealth()
    {
        yield return new WaitForSeconds(1f);
        while(true)
        {
            if (!generator.parent.gameObject.GetComponent<PlayerControl>().gameStopped)
            {
                yield return new WaitForSeconds(19.98f);
                startHealth += 50f;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

}
