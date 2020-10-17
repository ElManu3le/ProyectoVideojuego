using System;

/// <summary>
/// Interfaz para describir el comportamiento de un mecanismo que tiene
/// varios estados, discretos y que no necesariamente hacen al mecanismo
/// estar activo o no activo. Por ejemplo: una caja (palanca) de cambios
/// o un teclado numérico.
/// </summary>
public interface IMecanismoMultiestado : IMecanismo<Enum>
{
    /// <summary>Cuántos estados diferentes tiene el mecanismo.</summary>
    int num_estados { get; }

    /// <summary>En qué estado se encuentra el mecanismo.</summary>
    Enum estado_actual { get; }
}
