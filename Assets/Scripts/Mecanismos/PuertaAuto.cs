using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaAuto : MonoBehaviour
{
    public PlacaActivable mecanismo;

    [HideInInspector] public Transform p1, p2;

    [Range(0f, 2f)] public float smoothTime = .2f;

    private Vector3 pos_p1_abierta,
                    pos_p2_abierta,
                    pos_p1_cerrada,
                    pos_p2_cerrada;

    private void Start()
    {
        pos_p1_cerrada = p1.localPosition;
        pos_p1_abierta = pos_p1_cerrada + Vector3.left * 2.9f;
        pos_p2_cerrada = p2.localPosition;
        pos_p2_abierta = pos_p2_cerrada + Vector3.right * 2.9f;
        
        mecanismo.activado += abrir_puerta;
        mecanismo.desactivado += cerrar_puerta;
    }

    private bool abriendo = false;
    private bool cerrando = false;

    private Vector3 vel_ref_p1 = Vector3.zero,
                    vel_ref_p2 = Vector3.zero;
    private void FixedUpdate()
    {
        if (abriendo)
        {
            p1.localPosition = Vector3.SmoothDamp(p1.localPosition, pos_p1_abierta, ref vel_ref_p1, smoothTime);
            p2.localPosition = Vector3.SmoothDamp(p2.localPosition, pos_p2_abierta, ref vel_ref_p2, smoothTime);
            if (vel_ref_p1.magnitude * vel_ref_p2.magnitude < .0001f) abriendo = false;
        }
        else if (cerrando)
        {
            p1.localPosition = Vector3.SmoothDamp(p1.localPosition, pos_p1_cerrada, ref vel_ref_p1, smoothTime);
            p2.localPosition = Vector3.SmoothDamp(p2.localPosition, pos_p2_cerrada, ref vel_ref_p2, smoothTime);
            if (vel_ref_p1.magnitude * vel_ref_p2.magnitude < .0001f) cerrando = false;
        }
    }

    private void abrir_puerta()
    {
        abriendo = true;
        cerrando = false;
    }

    private void cerrar_puerta()
    {
        cerrando = true;
        abriendo = false;
    }

}
