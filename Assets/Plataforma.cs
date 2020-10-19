using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Plataforma : MonoBehaviour
{
    private Rigidbody blas = null;
    private Rigidbody este;
    private Vector3 pos_anterior_mundo,
                    pos_anterior_local;

    // Start is called before the first frame update
    void Start()
    {
        pos_anterior_mundo = transform.position;

        este = GetComponent<Rigidbody>();
        este.AddTorque(0, 1.6f, 0, ForceMode.VelocityChange);
    }
    Vector3 vel = Vector3.zero;
    private void FixedUpdate()
    {
        if (este.angularVelocity.y < 1.5f)
        {
            este.AddTorque(new Vector3(0, 1f, 0) * Time.fixedDeltaTime);
        }

        if (blas != null)
        {
            var delta_mundo = transform.position - pos_anterior_mundo;
            var delta_local = transform.InverseTransformPoint(blas.position) - pos_anterior_local;

            Debug.DrawRay(blas.transform.position, delta_mundo * 10, Color.blue, 10);
            Debug.DrawRay(blas.transform.position + delta_mundo * 10, delta_local * 10, Color.cyan, 10);

            blas.transform.position += delta_mundo + delta_local;

            pos_anterior_local = transform.InverseTransformPoint(blas.position);
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
