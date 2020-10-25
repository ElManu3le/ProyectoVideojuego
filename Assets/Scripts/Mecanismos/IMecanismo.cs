using System;

/// <summary>
/// Interfaz en plan ahí más genérica para describir mecanismos en general.
/// </summary>
/// <typeparam name="T">Tipo de dato que describe el estado del mecanismo.</typeparam>
public interface IMecanismo<T>
{
    /// <summary>El estado del mecanismo.</summary>
    T estado_actual { get; }
}
