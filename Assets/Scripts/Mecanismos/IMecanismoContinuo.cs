using System;

/// <summary>
/// Interfaz para describir el comportamiento de mecanismos cuyo estado
/// está mejor descrito con un valor continuo (un float en este caso).
/// Este tipo de mecanismos suelen tener un límite máximo y mínimo, como
/// por ejemplo una placa de presión o un tirador.
/// </summary>
public interface IMecanismoContinuo : IMecanismo<float>
{
    /// <summary>
    /// Evento que se dispara cuando el desplazamiento sobrepasa el límite.
    /// </summary>
    event Action limite_sobrepasado;

    /// <summary>Valor mínimo que puede alcanzar el «desplazamiento» del mecanismo.</summary>
    float min { get; }
    /// <summary>Valor máximo que puede alcanzar el «desplazamiento» del mecanismo.</summary>
    float max { get; }
    /// <summary>El límite es un valor entre el mínimo y el máximo que interesa saber cuándo se ha sobrepasado.</summary>
    float lim { get; }
}
