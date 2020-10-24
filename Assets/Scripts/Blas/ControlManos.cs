using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManos : MonoBlashaviour
{
    public static class Ajustes
    {
        public static float dist_extension_brazos { get; private set; }
        public static float max_vel_cambio_objetivo { get; private set; }
        public static float multiplicador_fuerza_blas { get; private set; }
        public static float lerp_pos_descanso_manos { get; private set; }
        static Ajustes()
        {
            dist_extension_brazos = 1.69f;
            max_vel_cambio_objetivo = .08f;
            multiplicador_fuerza_blas = 69f;
            lerp_pos_descanso_manos = .4f;
        }
    }

    private Mano mano_izquierda, mano_derecha;

    // Start is called before the first frame update
    private void Start()
    {
        mano_derecha = GameObject.Find("Mano Derecha").GetComponent<Mano>();
        mano_izquierda = GameObject.Find("Mano Izquierda").GetComponent<Mano>();
    }


    private void FixedUpdate()
    {
        mano_izquierda.comunicar_input(
            Blas.controles["extender_brazo_izquierdo"].ReadValue<float>(),
            Blas.controles["agarrar_mano_izquierda"].ReadValue<float>()
        );

        mano_derecha.comunicar_input(
            Blas.controles["extender_brazo_derecho"].ReadValue<float>(),
            Blas.controles["agarrar_mano_derecha"].ReadValue<float>()
        );
    }

}
