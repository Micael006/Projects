using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    Transform arrow;
    GameObject player;
    float Speed { get; set; }
    public bool isActing = true;
    public float distance;
    float angle;
    void Start()
    {
        Speed = 2f * DataHolder.fixedFrameTime;
        arrow = GetComponent<Transform>();
        gameObject.name = "arrow";
        player = GameObject.Find("Player");
        angle = 180f / Mathf.PI * Mathf.Atan((player.transform.position.y - arrow.position.y) / (player.transform.position.x - arrow.position.x));
        angle = (player.transform.position.x < arrow.transform.position.x) ? 180 + angle : angle;
    }

    void FixedUpdate()
    {
        if (isActing && !player.GetComponent<PlayerControl>().gameStopped)
        {
            if(Speed == 2f * DataHolder.fixedFrameTime)
            {
                StartCoroutine(SpeedUp());
            }
            distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - arrow.position.x, 2) + Mathf.Pow(player.transform.position.y - arrow.position.y, 2));
            arrow.position += new Vector3(Speed * Mathf.Cos(Mathf.PI * angle / 180f), Speed * Mathf.Sin(Mathf.PI * angle / 180f), 0);
            if (distance >= 30f)
            {
                isActing = false;
                Destroy(gameObject);
            }
        }
    }

    IEnumerator SpeedUp()
    {
        while(Speed < 4.5f * DataHolder.fixedFrameTime)
        {
            if (!player.GetComponent<PlayerControl>().gameStopped)
            {
                Speed += 0.5f * DataHolder.fixedFrameTime;
                yield return new WaitForSeconds(0.73f);
            }
            yield return new WaitForSeconds(0.02f);
        }
        Speed = Mathf.Min(Speed, 4.5f * DataHolder.fixedFrameTime);
    }
}
