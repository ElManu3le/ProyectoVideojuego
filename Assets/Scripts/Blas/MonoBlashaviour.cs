using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estoy hasta el gorro de los scripts de Blas
/// </summary>
public class MonoBlashaviour : MonoBehaviour
{
    protected Blas Blas { get; private set; }

    private void Awake()
    {
        Blas = FindObjectOfType<Blas>();
    }
}
