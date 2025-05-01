using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyHelper : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensor = new Vector3(1, 1, 1);
    }
}
