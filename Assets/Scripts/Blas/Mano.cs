using System;
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

    public Animator anim;

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

    private float smooth_time_actual = ControlManos.Ajustes.smooth_time_sin_input;

    // Input
    private float extension_brazo = 0f;
    private bool agarrando = false;

    private Collider candidato_al_agarre = null;
    private bool candidato_agarrado = false;
    float masa_agarrada = 0f;

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
        calcular_limites();

        calcular_objetivo_segun_input();

        mover_mano_hacia_objetivo();

        interactuar_con_objetos_de_la_escena();

        if (candidato_agarrado)
        {
            gestionar_objeto_agarrado();
        }

        animar_mano();

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

    private void animar_mano()
    {
        anim.SetBool("manocerrada", agarrando);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (candidato_al_agarre == null && other.attachedRigidbody != null)
        {
            candidato_al_agarre = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (candidato_al_agarre == other)
        {
            candidato_al_agarre = null;
            candidato_agarrado = false;
            Blas.cambiar_masa(-masa_agarrada);
            masa_agarrada = 0f;
        }
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

    private static Vector3 half_extents = new Vector3(.15f, .15f, .15f);
    private void calcular_limites()
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
    }

    private void calcular_objetivo_segun_input()
    {
        if (extension_brazo < .0001f)
        {
            objetivo = pos_descanso;
            smooth_time_actual = ControlManos.Ajustes.smooth_time_sin_input;
        }
        else
        {
            float des_pos = Vector3.Distance(pos_descanso, lim_posible_lejano),
                  des_lim = Vector3.Distance(pos_descanso, limite_lejano.position);

            if (des_pos < des_lim)
                objetivo = Vector3.Lerp(pos_descanso, lim_posible_lejano, extension_brazo + (des_lim - des_pos));
            else
                objetivo = Vector3.Lerp(pos_descanso, limite_lejano.position, extension_brazo);

            smooth_time_actual = ControlManos.Ajustes.smooth_time_con_input;
        }
    }

    private void mover_mano_hacia_objetivo()
    {
        //lerp_entre_pos_actual_y_objetivo = Mathf.MoveTowards(
        //    lerp_entre_pos_actual_y_objetivo, 
        //    extension, 
        //    ControlManos.Ajustes.max_vel_cambio_objetivo
        //);

        float dist = Vector3.Distance(objetivo, transform.position);
        //punto_mas_cercano = Mates.PuntoMasCercanoASegmento(
        //    transform.position,
        //    limite_lejano.position,
        //    limite_cercano.position
        //);
        //float dist_pmc = (Vector3.Distance(punto_mas_cercano, transform.position));
        //if (dist_pmc > .5f) transform.position = Vector3.MoveTowards(transform.position, punto_mas_cercano, dist_pmc - .5f);

        if (dist > float.Epsilon) 
            rb.MovePosition(Vector3.SmoothDamp(transform.position, objetivo, ref velocidad, smooth_time_actual));
    }

    private bool yoya = false;
    private bool un_ftg = false;
    private void interactuar_con_objetos_de_la_escena()
    {
        if (!agarrando) // Peñetazo/empujón
        {
            if (Physics.BoxCast(
                    transform.position,
                    half_extents,
                    transform.forward,
                    out RaycastHit info,
                    transform.rotation,
                    .1f,
                    Misc.ignorar_capas(Mano.capa, Blas.capa)
                ) &&
                info.collider.attachedRigidbody != null)
            {
                if (!yoya && !un_ftg && extension_brazo > .5f && velocidad.magnitude > 1.8f)
                { // Yoya
                    Debug.Log("Bro!!!!");
                    info.collider.attachedRigidbody.AddForceAtPosition(Vector3.ClampMagnitude(-info.normal * ControlManos.Ajustes.multiplicador_yoya_blas, info.collider.attachedRigidbody.mass * 5), info.point, ForceMode.Impulse);
                    yoya = true;
                }
                else if (velocidad.magnitude > .01f && extension_brazo > .01f)
                { // Empujar suavecito
                    Debug.Log("maaaan");
                    info.collider.attachedRigidbody.AddForceAtPosition(-info.normal * ControlManos.Ajustes.multiplicador_empuje_blas, info.point, ForceMode.Force);
                }
                un_ftg = true;
            }
            else // Na
            {
                yoya = un_ftg = false;
            }
        }
        else if (candidato_al_agarre != null && !candidato_agarrado) // Pulsado el boton de agarrar!!!!
        {
            if (candidato_al_agarre.attachedRigidbody.mass < dos_tercios_masa_blas)
            {
                candidato_al_agarre.transform.parent = transform;
                candidato_al_agarre.attachedRigidbody.useGravity = false;
                candidato_al_agarre.attachedRigidbody.isKinematic = true;
            }
            candidato_agarrado = true;
            masa_agarrada = candidato_al_agarre.attachedRigidbody.mass;
            Blas.cambiar_masa(candidato_al_agarre.attachedRigidbody.mass);
        }
    }

    private float dos_tercios_masa_blas = 69f / 3f * 2f;
    private void gestionar_objeto_agarrado()
    {
        if (agarrando)
        {
            try
            {
                candidato_al_agarre.attachedRigidbody.AddForce((transform.position - candidato_al_agarre.gameObject.transform.position).normalized * masa_agarrada * ControlManos.Ajustes.multiplicador_empuje_blas * (extension_brazo + 1f), ForceMode.Force);
            }
            catch (NullReferenceException)
            {
                candidato_agarrado = false;
                candidato_al_agarre = null;
                Blas.cambiar_masa(-masa_agarrada);
                masa_agarrada = 0f;
            }
        }
        else
        {
            if (candidato_al_agarre.attachedRigidbody.mass < dos_tercios_masa_blas)
            {
                candidato_al_agarre.transform.parent = null;
                candidato_al_agarre.attachedRigidbody.useGravity = true;
                candidato_al_agarre.attachedRigidbody.isKinematic = false;
            }
            candidato_agarrado = false;
            Blas.cambiar_masa(-masa_agarrada);
            masa_agarrada = 0f;
        }
    }

    //private void no(Collision collision)
    //{
    //    Debug.Log("oncollisionenter");
    //    en_contacto_con_algo = true;

    //    ContactPoint[] contactos = new ContactPoint[collision.contactCount];
    //    collision.GetContacts(contactos);

    //    var otro_rb = collision.collider.attachedRigidbody;
    //    if (otro_rb != null && !otro_rb.isKinematic)
    //    {   // Se le puede dar una yoya
    //        if (collision.contactCount > 0 && velocidad.magnitude > .1f)
    //        {
    //            foreach (var c in contactos)
    //            {
    //                otro_rb.AddForceAtPosition(
    //                    -c.normal * ControlManos.Ajustes.multiplicador_yoya_blas, 
    //                    c.point, 
    //                    ForceMode.Impulse
    //                );
    //            }
    //        }
    //    }

    //    objetivo = transform.position;
    //}

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
