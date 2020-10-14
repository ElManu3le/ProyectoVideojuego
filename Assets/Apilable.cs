using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


/// <summary>
/// Para hacer que los Rigidbody apilados uno encima de otro
/// comuniquen su masa noseque nosecuantas
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Apilable : MonoBehaviour
{
    // Para debugiar
    public TextMeshPro[] texto = new TextMeshPro[6];

    // Masas geteables publicamente
    public float masa_apilada { get; private set; }
#if UNITY_EDITOR
    // Queremos que en el editor salga un cacharrín para poder modificar el valor directamente
    public float masa_propia = 1.5f; // Odio esto con toda mi alma
#else
    public float masa_propia { get; private set; } // :(
#endif
    public float masa_total { get { return masa_propia + masa_apilada; } }

    // Datos a mano
    private Rigidbody rb;
    private int hash;
    private bool hay_que_actualizar_datos { get; set; }
    private bool hay_que_actualizar_texto { get; set; }

    /// <summary>
    /// Cada apilable se autorregistra en el diccionario con clave igual al
    /// <tt><see cref="UnityEngine.Object.GetHashCode()">Hash</see></tt> (un <tt>int</tt>)
    /// de su <tt>GameObject</tt>. Así, a la hora de comprobar si la colisión de las
    /// funciones <i>OnCollisionTal(Collider otro)</i> es contra otro apilable,
    /// no tenemos que llamar a la función <i>GetComponent</i> (que es, según
    /// tengo entendido, muy costosa), podemos simplemente mirar en el diccionario.
    /// </summary>
    private static Dictionary<int, Apilable> apilables = new Dictionary<int, Apilable>();

    /// <summary>
    /// Cada apilable guarda un diccionario de referencias a otros apilables sobre
    /// los que está apoyado. Esto es una diccionario con clave int (hash del apilable) 
    /// y tuplas de tres elementos: el apilable que tiene debajo, el (<tt>int</tt>)
    /// número de contactos contra ese apilable y (<tt>double</tt>) el porcentaje de 
    /// la masa comunicado haciaabajo. La suma de todos los porcentajes de todas las 
    /// tuplas de la lista de un apilable debe estar siempre entre 0 y 1.
    /// </summary>
    private Dictionary<int, (Apilable apilable, 
                             int      num_contactos, 
                             float    masa_transmitida)> apoyos = new Dictionary<int, (Apilable, int, float)>();

    /// <summary>
    /// Para distribuir la masa cuando parte está en contacto con colliders normales
    /// y parte está en contacto con apilables.
    /// </summary>
    private int otros_puntos_de_apoyo
    { 
        get => __otros_p;
        set => __otros_p = value <= 0 ? 0 : value;
    }
    private int __otros_p = 0; // Variable privada para la propiedad «otros_puntos_de_apoyo»

    /// <summary>
    /// El total de puntos de apoyo se consigue sumando todos los puntos de la lista
    /// de apoyos con los puntos de apoyo que no pertenecen a apilables.
    /// </summary>    
    private int total_puntos_de_apoyo 
    { 
        get 
        {
            int acum_ptos = 0;
            foreach (var par_clave_tupla in apoyos) 
                acum_ptos += par_clave_tupla.Value.num_contactos;
            return acum_ptos + otros_puntos_de_apoyo;
        } 
    }

    // En start registra el apilable y rellena datos
    private void Start()
    {
        // Autorregistro en el diccionario
        apilables.Add(hash = gameObject.GetHashCode(), this);

        // Setear datos
        rb = GetComponent<Rigidbody>();
        masa_apilada = 0f;
        masa_propia = rb.mass;
    }

    // Actualizar datos
    private void FixedUpdate()
    {
        if (hay_que_actualizar_datos) actualizar_datos();
    }

    // Actualizar visuales (debug)
    private void Update()
    {
        if (hay_que_actualizar_texto) actualizar_texto();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Comprueba si el otro collider es apilable
        if (apilables.TryGetValue(collision.gameObject.GetHashCode(), out Apilable otro))
        {
            
            // Nos guardamos los contactos en un array
            ContactPoint[] contactos = new ContactPoint[collision.contactCount];
            collision.GetContacts(contactos);

            // Un contacto válido es el que se produce con un apilable que está debajo
            int num_contactos_validos = 0;

            foreach (ContactPoint punto in contactos)
            {
                if (Vector3.Angle(Vector3.up, punto.normal) <= 90 - float.Epsilon)
                {
                    // La normal de la colisión difiere en casi 90 grados o menos de Vector3.up
                    // (o sea, el otro collider está debajo)
                    num_contactos_validos++;
                    Debug.DrawRay(punto.point, punto.normal * .5f, Color.red, 2);
                }
            }

            if (num_contactos_validos > 0)
            {
                // Este apilable se apunta que tiene a otro ahí debajo
                apoyos.Add(otro.hash, (otro, num_contactos_validos, 0f /* se calcula luego */));
                hay_que_actualizar_datos = true;
            }
        }
        else
        {
            // Esto no frunga
            // otros_puntos_de_apoyo += collision.contactCount;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Comprueba si el otro collider es apilable
        if (apilables.TryGetValue(collision.gameObject.GetHashCode(), out Apilable otro))
        {
            if (apoyos.TryGetValue(otro.hash, out (Apilable ap, int ctos, float masa) tupla))
            {

                otro.masa_apilada -= tupla.masa;
                otro.actualizar_datos();
                apoyos.Remove(otro.hash);
                hay_que_actualizar_datos = true;
            }
        }
        else
        {
            // Pffffffffff
            // otros_puntos_de_apoyo -= collision.contactCount;
        }
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.mass = masa_propia;
        actualizar_texto();
    }

    public void actualizar_datos()
    {
        int contactos_de_este = total_puntos_de_apoyo;
        float masa_de_este = masa_total;

        List<int> claves = new List<int>(apoyos.Keys);
        for (int i = 0; i < claves.Count; ++i)
        {
            int hash = claves[i];
            var (el_otro, num_contactos, masa_antigua) = apoyos[hash];
            apoyos[hash] = (
                el_otro,
                num_contactos,
                (float)num_contactos / contactos_de_este * masa_de_este
            );

            float masa_delta = apoyos[hash].masa_transmitida - masa_antigua;
            apoyos[hash].apilable.masa_apilada += masa_delta;
            apoyos[hash].apilable.actualizar_datos();
        }

        hay_que_actualizar_datos = false;
        hay_que_actualizar_texto = true;
    }

    ///<summary><b>(DEBUG)</b> - Actualiza los TextMeshPro para que casen con los valores reales</summary>
    public void actualizar_texto()
    {
        foreach (TextMeshPro t in texto)
            t.text = masa_propia + "\n" + masa_apilada;
        hay_que_actualizar_texto = false;
    }
}
