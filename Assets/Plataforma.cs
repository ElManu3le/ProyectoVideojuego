using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Plataforma : MonoBehaviour
{
    public Vector3 veloscidas = Vector3.zero;

    private Rigidbody blas = null;
    private Rigidbody este;
    private Vector3 pos_anterior_mundo;

    // Start is called before the first frame update
    void Start()
    {
        pos_anterior_mundo = transform.position;

        este = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (este.velocity.sqrMagnitude < veloscidas.sqrMagnitude && veloscidas != Vector3.zero)
        {
            este.AddForce(veloscidas * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        if (blas != null)
        {
            var delta_mundo = transform.position - pos_anterior_mundo;
            blas.position += delta_mundo;
        }
        pos_anterior_mundo = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.name == "Blas")
        {
            blas = collision.collider.attachedRigidbody;
            Debug.Log("Blas en plataforma");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (blas != null)
        {
            blas = null;
            Debug.Log("Blas no en plataforma");
        }
    }
}
