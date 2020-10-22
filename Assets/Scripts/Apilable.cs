using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Para hacer que los Rigidbody apilados uno encima de otro
/// comuniquen su masa noseque nosecuantas
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Apilable : MonoBehaviour
{
    // Datos a mano
    protected Rigidbody rb;
    private int hash;

    /// <summary>
    /// Evento al que se pueden suscribir elementos que quieren 
    /// enterarse de los cambios en una pila de objetos apilables.
    /// </summary>
    public event Action<int, float> fin_de_propagacion;

    /// <summary>
    /// Masa que el apilable está recibiendo de otros apilables
    /// que tiene encima.
    /// </summary>
    public float masa_apilada { get; private set; }

    /// <summary>La masa del apilable, por su cuenta. Es «constante».</summary>
#if UNITY_EDITOR
    // Queremos que en el editor salga un cacharrín para poder modificar el valor directamente
    [Min(0f)] public float masa_propia = 1.5f; // Odio esto con toda mi alma
#else
    // Esto, en teoría, es lo que se incluye en la «build» de verdad.
    public float masa_propia { get; private set; } // :(
#endif

    /// <summary>
    /// La parte de la masa total que el apilable <b>NO</b> está transmitiendo
    /// a otros apilables.
    /// </summary>
    public float masa_restante { get; private set; }

    /// <summary>
    /// La suma de la masa propia más la masa apilada.
    /// </summary>
    public float masa_total { get { return masa_propia + masa_apilada; } }

    /// <summary>Flag para actualizar masas y propagar cambios.</summary>
    private bool hay_que_actualizar_datos { get; set; }

    /// <summary>Flag para cambiar el texto.</summary>
    protected bool hay_que_actualizar_texto { get; set; }

    /// <summary>
    /// Cada apilable se autorregistra en el diccionario con clave igual al
    /// <tt><see cref="UnityEngine.Object.GetHashCode()">Hash</see></tt> (un <tt>int</tt>)
    /// de su <tt>GameObject</tt>. Así, a la hora de comprobar si la colisión de las
    /// funciones <i>OnCollisionTal(Collider otro)</i> es contra otro apilable,
    /// no tenemos que llamar a la función <i>GetComponent</i> (que es, según
    /// tengo entendido, muy costosa), podemos simplemente mirar en el diccionario.
    /// </summary>
    protected static Dictionary<int, Apilable> apilables = new Dictionary<int, Apilable>();

    /// <summary>
    /// Cada apilable guarda un diccionario de referencias a otros apilables sobre
    /// los que está apoyado. Esto es una diccionario con clave int (hash del apilable) 
    /// y tuplas de tres elementos: el apilable que tiene debajo, el (<tt>int</tt>)
    /// número de contactos contra ese apilable y (<tt>double</tt>) el porcentaje de 
    /// la masa comunicado haciaabajo. La suma de todos los porcentajes de todas las 
    /// tuplas de la lista de un apilable debe estar siempre entre 0 y 1.
    /// </summary>
    private Dictionary<int, (Apilable apilable,
                             int num_contactos,  // Una serie de tipos entre paréntesis forman una tupla
                             float masa_transmitida)> apoyos = new Dictionary<int, (Apilable, int, float)>();

    /// <summary>
    /// Para las colisiones que no sean con apilable usamos otro diccionario, para
    /// poder así distribuir la masa con propiedad. Creo.
    /// </summary>
    private Dictionary<int, (Collider col,
                             int num_contactos)> otros_apoyos = new Dictionary<int, (Collider, int)>();

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
            foreach (var par_clave_tupla in otros_apoyos)
                acum_ptos += par_clave_tupla.Value.num_contactos;
            return acum_ptos;
        }
    }

    // En start registra el apilable y rellena datos
    private void Start()
    {
        // Autorregistro en el diccionario
        hash = super_hash();
        if (!apilables.ContainsKey(hash)) apilables.Add(hash, this);

        // Setear datos
        rb = GetComponent<Rigidbody>();
        masa_apilada = 0f;
        masa_propia = rb.mass;
        masa_restante = 0f;
    }

    // Actualizar datos
    private void FixedUpdate()
    {
        if (hay_que_actualizar_datos) actualizar_datos();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nos guardamos los contactos en un array
        ContactPoint[] contactos = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactos);

        // Un contacto válido es el que se produce con un collider que está debajo
        int num_contactos_validos = 0;

        foreach (ContactPoint punto in contactos)
        {
            const float epsilon = 2 * float.Epsilon;
            if (Vector3.Angle(Vector3.up, punto.normal) < 90f - epsilon)
            {
                // La normal de la colisión difiere en casi 90 grados o menos de Vector3.up
                // (o sea, el otro collider está debajo)
                num_contactos_validos++;
                Debug.DrawRay(punto.point, punto.normal * .5f, Color.red, 2);
            }
        }

        if (num_contactos_validos > 0)
        {
            int hash = collision.gameObject.GetHashCode();
            // Comprueba si el otro collider es apilable
            if (apilables.TryGetValue(hash, out Apilable otro))
            {
                if (otro.apoyos.ContainsKey(this.hash))
                {
                    // No pueden estar los dos a la vez debajo del otro!!!!
                    // Es un error, probablemente están al lado. Hay que eliminar
                    // esta colisión del diccionario
                    otro.apoyos.Remove(this.hash);
                    otro.hay_que_actualizar_datos = true;
                }
                else
                {
                    try
                    {
                        // Este apilable se apunta que tiene a otro ahí debajo
                        apoyos.Add(otro.hash, (otro, num_contactos_validos, 0f /* se calcula luego */));
                    }
                    catch (ArgumentException) { } // Catcheamos esto por culpa de Blas
                }
            }
            else
            {
                // Este apilable tiene debajo un collider que no es apilable.
                otros_apoyos.Add(hash, (collision.collider, num_contactos_validos));
            }

            hay_que_actualizar_datos = true;
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
            }
        }
        else
        {
            int hash = collision.gameObject.GetHashCode();
            if (otros_apoyos.ContainsKey(hash))
            {
                otros_apoyos.Remove(hash);
            }
        }

        hay_que_actualizar_datos = true;
    }

    /// <summary>
    /// No sé por qué tengo que hacer esto!!!! Si no es overrideado en superclases,
    /// se invoca desde Apilable!!!! Pero no quiero el hash de apilable, quiero el
    /// hash de la superclase. Y eso, al parecer, solo se puede conseguir overrideando
    /// este método con el mismo código, exactamente.
    /// </summary>
    /// <returns>Hash del gameObject</returns>
    protected virtual int super_hash()
    {
        return gameObject.GetHashCode();
    }

    /// <summary>
    /// Si el evento fin_de_propagacion tiene suscriptores, lo invoca.
    /// </summary>
    private void comunicar_fin_de_propagacion()
    {
        fin_de_propagacion?.Invoke(hash, masa_restante);
    }

    /// <summary>
    /// Reparte la masa propia y la que se ha propagado entre todos los collider que tiene
    /// debajo, según el número de puntos de contacto. Si uno o más collider no son apilables,
    /// lanza el evento de fin_de_propagacion. Además, baja la flag hay_que_actualizar_datos
    /// y sube hay_que_actualizar_texto.
    /// </summary>
    public void actualizar_datos()
    {
        int contactos_de_este = total_puntos_de_apoyo;
        float masa_de_este = masa_total;
        float total_masa_transmitida = 0f;

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
            total_masa_transmitida += apoyos[hash].masa_transmitida;
            float masa_delta = apoyos[hash].masa_transmitida - masa_antigua;
            apoyos[hash].apilable.masa_apilada += masa_delta;
            apoyos[hash].apilable.actualizar_datos();
        }

        masa_restante = masa_total - total_masa_transmitida;
        if (otros_apoyos.Count > 0) comunicar_fin_de_propagacion();

        hay_que_actualizar_datos = false;
        hay_que_actualizar_texto = true;
    }

    /// <summary>
    /// Método para comprobar, desde fuera de la clase, si un
    /// GameObject tiene un componente apilable.
    /// </summary>
    /// <param name="hash_de_gameobject">
    /// Entero devuelto por <tt>
    ///     <see cref="UnityEngine.Object.GetHashCode()">Object.GetHashCode()</see>
    /// </tt>
    /// </param>
    /// <returns>El apilable, si se ha encontrado, o null.</returns>
    public static bool es_apilable(int hash_de_gameobject, out Apilable apilable)
    {
        return apilables.TryGetValue(hash_de_gameobject, out apilable);
    }
}
