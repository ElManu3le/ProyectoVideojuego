using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManos : MonoBehaviour
{
    public InputActionAsset controles;
    public float dist_extension_brazos = 1.3f;
    public Transform pos_ideal_cerca_der, pos_ideal_cerca_izq,
                     pos_ideal_lejos_der, pos_ideal_lejos_izq;

    private Rigidbody mano_der, mano_izq;
   
    private Vector3 objetivo_der, objetivo_izq;

    // Start is called before the first frame update
    private void Start()
    {
        mano_der = GameObject.Find("Mano Derecha").GetComponent<Rigidbody>();
        mano_izq = GameObject.Find("Mano Izquierda").GetComponent<Rigidbody>();
        
        objetivo_der = pos_ideal_cerca_der.position;
        objetivo_izq = pos_ideal_cerca_izq.position;
    }

    private void Update()
    {
        float ebi = controles["extender_brazo_izquierdo"].ReadValue<float>();
        objetivo_izq = Vector3.Lerp(pos_ideal_cerca_izq.position, pos_ideal_lejos_izq.position, ebi);
        float dist = Vector3.Distance(objetivo_izq, mano_izq.position);
        if (dist > .05f) mano_izq.AddForce((objetivo_izq - mano_izq.position) * 100);
        else mano_izq.velocity = Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos_ideal_cerca_izq.position, pos_ideal_lejos_izq.position);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(objetivo_izq, .07f);
    }
}
