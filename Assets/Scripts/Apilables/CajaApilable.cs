using TMPro;
using UnityEngine;

/// <summary>
/// Una caja apilable. Es apilable, y además es una caja.
/// </summary>
[ExecuteInEditMode]
public sealed class CajaApilable : Apilable 
    // «Sealed» para que no se pueda heredar de CajaApilable.
    // Nos interesa que la herencia sea desde la base, Apilable.
{
    /// <summary>
    /// Esto se asigna desde el editor!!!!!
    /// </summary>
    public TextMeshPro[] texto = null;

    /// <summary>
    /// Esto es para que los cambios en el editor se actualicen inmediatamente, creo.
    /// </summary>
    private void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.mass = masa_propia; // igual esto hay que moverlo a la clase base. pero pf
        actualizar_texto();
    }

// El texto solo lo actualiza, en tiempo real, dentro del editor. En el juego
// en sí, se supone que eso no pasará.
#if UNITY_EDITOR
    private void Update()
    {
        if (hay_que_actualizar_texto &&
            texto != null &&
            texto.Length > 0)
        {
            actualizar_texto();
        }
    }
#endif

    ///<summary>
    ///Actualiza los TextMeshPro para que casen con los valores reales.
    ///</summary>
    public void actualizar_texto()
    {
        foreach (TextMeshPro t in texto)
            t.text = masa_propia.ToString("N1"); // N1 para que solo escriba un decimal.
        hay_que_actualizar_texto = false;
    }

    protected override int super_hash()
    {
        return gameObject.GetHashCode();
    }
}
