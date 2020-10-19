using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlacaActivable : MonoBehaviour, IMecanismoActivable
{
    [Min(0f)] public float peso_necesario = 15f;

    private Dictionary<int, float> apilables = new Dictionary<int, float>();

    public event Action /*IMecanismoActivable*/activado;
    public event Action /*IMecanismoActivable*/desactivado;

    public int num_apilados { get { return apilables.Count; } }
    public float peso_soportado { get; private set; }
    public bool estado_actual { get; private set; }

    private void Start()
    {
        peso_soportado = 0f;
        estado_actual = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        int hash = collision.gameObject.GetHashCode();
        if (Apilable.es_apilable(hash, out var apilable))
        {
            apilable.fin_de_propagacion += callback_apilable;
            apilables.Add(hash, apilable.masa_restante);
            peso_soportado += apilable.masa_restante;

            invocar_evento_segun_estado();
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
            if (num_apilados == 0) peso_soportado = 0f;

            invocar_evento_segun_estado();
        }
    }

    private void callback_apilable(int hash, float nueva_masa)
    {
        peso_soportado -= apilables[hash];
        apilables[hash] = nueva_masa;
        peso_soportado += apilables[hash];

        invocar_evento_segun_estado();
    }

    private void invocar_evento_segun_estado()
    {
        if (!estado_actual && peso_soportado > peso_necesario)
        { // Desactivado -> Activado
            estado_actual = true;
            activado?.Invoke();
        }
        else if (estado_actual && peso_soportado < peso_necesario)
        { // Activado -> Desactivado
            estado_actual = false;
            desactivado?.Invoke();
        }
    }

}
