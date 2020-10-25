using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalancaCincoEstados : MonoBehaviour, IMecanismoMultiestado<PalancaCincoEstados.Estado>
    // Seguro que esto se puede hacer mejor. Supongo que se
    // necesitarán clases abstractas en vez de interfaces.
{
    public enum Estado
    {
        Atras2   = -2,
        Atras1   = -1,
        Neutro    = 0,
        Adelante1 = 1,
        Adelante2 = 2
    }

    public Estado estado_actual { get; private set; }

    public int num_estados => Enum.GetValues(typeof(Estado)).Length;

    public event Action cambio_de_estado;

    private Rigidbody rb_palo;

    private void Start()
    {
        rb_palo = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
       
    }
}
