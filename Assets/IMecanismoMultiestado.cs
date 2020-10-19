using System;

/// <summary>
/// Interfaz para describir el comportamiento de un mecanismo que tiene
/// varios estados, discretos y que no necesariamente hacen al mecanismo
/// estar activo o no activo. Por ejemplo: una caja (palanca) de cambios
/// o un teclado numérico.
/// </summary>
public interface IMecanismoMultiestado : IMecanismo<Enum>
{
    /// <summary>
    /// Evento que se dispara cuando el mecanismo cambia de un estado a otro diferente.
    /// </summary>
    event Action cambio_de_estado;

    /// <summary>Cuántos estados diferentes tiene el mecanismo.</summary>
    int num_estados { get; }
}
