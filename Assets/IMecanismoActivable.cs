using System;

/// <summary>
/// Interfaz para describir el comportamiento de mecanismos con dos
/// estados diferentes (que no tienen que ser los únicos): activo y
/// no activo. Por ejemplo, un botón o un(a palanca de un) cambio de
/// vías.
/// </summary>
public interface IMecanismoActivable : IMecanismo<bool>
{
    /// <summary>Evento a lanzar cuando el mecanismo pase a estar activado.</summary>
    event Action activado;
    /// <summary>Evento a lanzar cuando el mecanismo pase a estar desactivado.</summary>
    event Action desactivado;

    /// <summary>Si el mecanismo se puede considerar activado o no.</summary>
    bool esta_activado { get; }
}
