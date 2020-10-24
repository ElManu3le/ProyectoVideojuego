using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Mano : MonoBlashaviour
{
    /// <summary>
    /// Máscara de la capa Manos (capa nº 11)
    /// </summary>
    public static readonly LayerMask capa = 1 << 11;

    public Transform limite_cercano, limite_lejano;
    /// <summary>
    /// Lerp entre limite_cercano y limite_lejano
    /// </summary>
    private Vector3 pos_descanso = Vector3.zero;

    /// <summary>
    /// El Rigidbody que va a tener obligatoriamente la mano.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// El punto en el espacio hacia el que se mueve la mano.
    /// Cambia suavemente entre la posición actual de la mano
    /// y la interpolación lineal de las posiciones ideales 
    /// según los controles.
    /// </summary>
    private Vector3 objetivo;

    /// <summary>
    /// La posición más lejana que puede alcanzar la mano. Originalmente
    /// pos_ideal_lejos.position, pero estará más cerca si hay un collider
    /// entre medio.
    /// </summary>
    private Vector3 lim_posible_lejano;

    /// <summary>
    /// Variable ref para SmoothDamp. Usada para golpear cosas.
    /// </summary>
    private Vector3 velocidad;

    /// <summary>
    /// Pffffffffffffffffffffffffffffffffffffffffffffffffff
    /// </summary>
    private Vector3 punto_mas_cercano = Vector3.zero;

    /// <summary>
    /// PPPPPPPPPPPPFffffffffffffffffffffffffffFFFFFFFFFFFFf
    /// </summary>
    private float lerp_entre_pos_actual_y_objetivo = 0f;


    // Input
    private float extension_brazo = 0f;
    private bool agarrando = false;

    // No sé
    private bool en_contacto_con_algo = false,
                 va_a_collidear = false;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        velocidad = Vector3.zero;

        lim_posible_lejano = limite_lejano.position = 
                limite_cercano.position + transform.forward * ControlManos.Ajustes.dist_extension_brazos;

        pos_descanso = objetivo = transform.position = 
                Vector3.Lerp(limite_cercano.position, limite_lejano.position, ControlManos.Ajustes.lerp_pos_descanso_manos);
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        recalcular_objetivos();
        rb.MovePosition(Vector3.SmoothDamp(transform.position, objetivo, ref velocidad, .005f));

        //mover();

        //if (rb.SweepTest(transform.forward, out RaycastHit info, velocidad.magnitude))
        //{
        //    objetivo = transform.position + Vector3.forward * info.distance;
        //    objetivo += transform.forward * -.5f;
        //    va_a_collidear = true;
        //}
        //else
        //{
        //    va_a_collidear = false;
        //}
    }

    /// <summary>
    /// La mano no sabe si es la izquierda o la derecha, así que ControlManos tiene
    /// que pasarle el input que le corresponda.
    /// </summary>
    /// <param name="extension_brazo">nosequé</param>
    /// <param name="agarrando">nosecuántas</param>
    public void comunicar_input(float extension_brazo, float agarrando)
    {
        this.extension_brazo = extension_brazo;
        this.agarrando = agarrando > .5f;
    }

    private static Vector3 half_extents = new Vector3(.15f,.15f,.15f);
    private void recalcular_objetivos()
    {
        pos_descanso = Vector3.Lerp(limite_cercano.position, limite_lejano.position, ControlManos.Ajustes.lerp_pos_descanso_manos);
        lim_posible_lejano = limite_lejano.position;

        // Palante
        if (Physics.BoxCast(
                limite_cercano.position,
                half_extents,
                limite_cercano.forward,
                out RaycastHit info,
                transform.rotation,
                ControlManos.Ajustes.dist_extension_brazos,
                Misc.ignorar_capas(Mano.capa, Blas.capa)
           ))
        {
            lim_posible_lejano = limite_cercano.position + (transform.forward * info.distance);
            if (Vector3.Distance(limite_cercano.position, lim_posible_lejano) <
                Vector3.Distance(limite_cercano.position, pos_descanso))
            {
                pos_descanso = lim_posible_lejano;
            }
        }
        //else
        //{
        //    lim_posible_lejano = limite_lejano.position;
        //    pos_descanso = Vector3.Lerp(limite_cercano.position, limite_lejano.position, ControlManos.Ajustes.lerp_pos_descanso_manos);
        //}

        objetivo = pos_descanso;
    }

    /// <summary>
    /// Esto está regulero. Mueve la mano
    /// </summary>
    /// <param name="extension">input</param>
    private void mover()
    {
        //lerp_entre_pos_actual_y_objetivo = Mathf.MoveTowards(
        //    lerp_entre_pos_actual_y_objetivo, 
        //    extension, 
        //    ControlManos.Ajustes.max_vel_cambio_objetivo
        //);
        if (!en_contacto_con_algo && !va_a_collidear)
            objetivo = Vector3.Lerp(
                pos_descanso,
                limite_lejano.position,
                lerp_entre_pos_actual_y_objetivo
            );
        //float dist = Vector3.Distance(objetivo, transform.position);

        //punto_mas_cercano = Mates.PuntoMasCercanoASegmento(
        //    transform.position, 
        //    pos_ideal_cerca.position, 
        //    pos_ideal_lejos.position
        //);
        //float dist_pmc = (Vector3.Distance(punto_mas_cercano, transform.position));

        ////if (dist_pmc > .5f) transform.position = Vector3.MoveTowards(transform.position, punto_mas_cercano, dist_pmc - .5f);
        //if (dist > float.Epsilon) transform.position = Vector3.SmoothDamp(transform.position, objetivo, ref velocidad, .001f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("oncollisionenter");
        en_contacto_con_algo = true;

        ContactPoint[] contactos = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactos);

        var otro_rb = collision.collider.attachedRigidbody;
        if (otro_rb != null && !otro_rb.isKinematic)
        {   // Se le puede dar una yoya
            if (collision.contactCount > 0 && velocidad.magnitude > .1f)
            {
                foreach (var c in contactos)
                {
                    otro_rb.AddForceAtPosition(
                        -c.normal * ControlManos.Ajustes.multiplicador_fuerza_blas, 
                        c.point, 
                        ForceMode.Impulse
                    );
                }
            }
        }

        objetivo = transform.position;
    }

    private void OnCollisionExit(Collision collision)
    {
        en_contacto_con_algo = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(limite_cercano.position, limite_lejano.position);
        Gizmos.DrawSphere(lim_posible_lejano, .085f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(objetivo, .07f);
    }
#endif
}
