using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Placa : MonoBehaviour, IMecanismoContinuo, IMecanismoActivable
{
    public float peso_soportado { get; private set; }
    [Min(0f)] public float peso_necesario = 15f;

    private int num_apilados = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (Apilable.es_apilable(collision.gameObject.GetHashCode(), out var apilable))
        {
            apilable.fin_de_propagacion += callback_apilable;
            Debug.Log("Evento añadido a la placa.");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (Apilable.es_apilable(collision.gameObject.GetHashCode(), out var apilable))
        {
            apilable.fin_de_propagacion -= callback_apilable;
            Debug.Log("Evento quitado de la placa.");
        }
    }

    private void callback_apilable()
    {

    }
}
