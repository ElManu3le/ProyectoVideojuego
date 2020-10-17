using System;

/// <summary>
/// Interfaz en plan ahí más genérica para describir mecanismos en general.
/// </summary>
/// <typeparam name="T">Tipo de dato que describe el estado del mecanismo.</typeparam>
public interface IMecanismo<T>
{
    /// <summary>
    /// Evento a lanzar cuando el mecanismo cambia (significativamente) de estado.
    /// Por ejemplo, un cambio significativo es cuando un botón pasa de estar no
    /// presionado a estar presionado. Un cambio no significativo es cuando un botón
    /// se pulsa ligeramente sin llegar a activarlo.
    /// </summary>
    event Action<T> cambio_de_estado;
}
