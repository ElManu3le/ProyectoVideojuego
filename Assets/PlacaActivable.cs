using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Una placa de presión con un peso necesario para estar activada.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlacaActivable : MonoBehaviour, IMecanismoActivable
{
    /// <summary>Peso mínimo que tiene que soportar la placa para activarse.</summary>
    [Min(0f)] public float peso_necesario = 15f;
    public TextMeshPro texto = null;

    /// <summary>
    /// Apilables en contacto directo con la placa.
    /// </summary>
    private Dictionary<int, float> apilables = new Dictionary<int, float>();

    public event Action /*IMecanismoActivable*/activado;
    public event Action /*IMecanismoActivable*/desactivado;

    public int num_apilados { get { return apilables.Count; } }
    public float peso_soportado { get; private set; }

    /// <summary>Si está activada (true) o no (false).</summary>
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

    /// <summary>
    /// Método que se llama cuando un apilable al que está suscrito lanza
    /// el evento fin_de_propagacion. Actualiza el peso soportado por la placa
    /// y el estado actual, y lanza eventos según si se activa o desactiva.
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="nueva_masa"></param>
    private void callback_apilable(int hash, float nueva_masa)
    {
        peso_soportado -= apilables[hash];
        apilables[hash] = nueva_masa;
        peso_soportado += apilables[hash];

        invocar_evento_segun_estado();
    }

    /// <summary>
    /// Actualiza el estado (activado/desactivado) y
    /// lanza los eventos homónimos según corresponda.
    /// </summary>
    private void invocar_evento_segun_estado()
    {
        if (texto != null) texto.text = peso_soportado.ToString("N2") + " kg";

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
