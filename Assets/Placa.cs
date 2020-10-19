using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Placa : MonoBehaviour, IMecanismoContinuo, IMecanismoActivable
{
    public float peso_soportado { get; private set; }
    private bool hay_que_recalcular_peso { get; set; }
    [Min(0f)] public float peso_necesario = 15f;

    private Dictionary<int, float> apilables = new Dictionary<int, float>();

    public event Action /*IMecanismoContinuo*/limite_sobrepasado;
    public event Action /*IMecanismoActivable*/activado;
    public event Action /*IMecanismoActivable*/desactivado;

    public int num_apilados { get { return apilables.Count; } }

    public float /*IMecanismoContinuo*/min => throw new NotImplementedException();

    public float /*IMecanismoContinuo*/max => throw new NotImplementedException();

    public float /*IMecanismoContinuo*/lim => throw new NotImplementedException();

    float IMecanismo<float>.estado_actual { get => desplazamiento_placa; }
    private float desplazamiento_placa;

    bool IMecanismo<bool>.estado_actual { get => placa_activada; }
    private bool placa_activada;

    private void FixedUpdate()
    {
        Debug.Log(peso_soportado);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int hash = collision.gameObject.GetHashCode();
        if (Apilable.es_apilable(hash, out var apilable))
        {
            apilable.fin_de_propagacion += callback_apilable;
            apilables.Add(hash, apilable.masa_restante);
            peso_soportado += apilable.masa_restante;
            Debug.Log("Evento añadido a la placa.");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int hash = collision.gameObject.GetHashCode();
        if (Apilable.es_apilable(hash, out var apilable))
        {
            apilable.fin_de_propagacion -= callback_apilable;
            apilables.Remove(hash);
            peso_soportado -= apilable.masa_restante;
            Debug.Log("Evento quitado de la placa.");
            if (num_apilados == 0) peso_soportado = 0f;
        }
    }

    private void callback_apilable(int hash, float nueva_masa)
    {
        peso_soportado -= apilables[hash];
        apilables[hash] = nueva_masa;
        peso_soportado += apilables[hash];
    }


}
