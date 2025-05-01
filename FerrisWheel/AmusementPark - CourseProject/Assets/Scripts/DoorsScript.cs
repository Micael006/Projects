using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DoorsScript : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float doorWidth;
    bool doorsClosed = true;
    int maxCount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "OpenDoorsZone" && doorsClosed)
        {
            StartCoroutine(OpenDoors());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!doorsClosed)
        {
            StartCoroutine(CloseDoors());
        }
    }

    IEnumerator OpenDoors()
    {
        int count = 0;
        doorsClosed = false;
        while (count < maxCount)
        {
            leftDoor.transform.Translate(-doorWidth / maxCount, 0, 0);
            rightDoor.transform.Translate(doorWidth / maxCount, 0, 0);
            count++;
            yield return new WaitForSeconds(2.5f / maxCount);
        }
    }

    IEnumerator CloseDoors()
    {
        int count = 0;
        doorsClosed = true;
        while (count < maxCount)
        {
            leftDoor.transform.Translate(doorWidth / maxCount, 0, 0);
            rightDoor.transform.Translate(-doorWidth / maxCount, 0, 0);
            count++;
            yield return new WaitForSeconds(2.5f / maxCount);
        }
    }
}
