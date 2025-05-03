using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeController : MonoBehaviour
{
    Transform blade;
    GameObject player;
    float Speed { get; set; }
    public float distance;
    public Vector3 startPos;
    public float angle;
    void Start()
    {
        Speed = 6f * DataHolder.fixedFrameTime;
        blade = GetComponent<Transform>();
        player = GameObject.Find("Player");
        angle = blade.rotation.eulerAngles.z;
        startPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player.GetComponent<PlayerControl>().gameStopped)
        {
            distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - startPos.x, 2) + Mathf.Pow(transform.position.y - startPos.y, 2));
            blade.position += new Vector3(Speed * Mathf.Cos(Mathf.PI * angle / 180f), Speed * Mathf.Sin(Mathf.PI * angle / 180f), 0);
            if (distance >= 6f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "elfArcher")
        {
            collision.gameObject.GetComponent<ElfArcherController>().GotHurt();
            Destroy(gameObject);
        }
    }
}
