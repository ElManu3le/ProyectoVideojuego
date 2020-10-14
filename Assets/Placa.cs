using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placa : MonoBehaviour
{
    float masatotal = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity);
    }

    Vector3 ant = Vector3.zero;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.impulse != Vector3.zero)
        {
            ant = collision.impulse;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("exit");
    }
}
