using System;

/// <summary>
/// Interfaz para describir el comportamiento de un mecanismo que tiene
/// varios estados, discretos y que no necesariamente hacen al mecanismo
/// estar activo o no activo. Por ejemplo: una caja (palanca) de cambios
/// o un teclado numérico.
/// </summary>
public interface IMecanismoMultiestado
{
    /// <summary>
    /// Cuántos estados diferentes tiene el mecanismo.
    /// </summary>
    /// <returns>Un entero mayor que cero.</returns>
    int num_estados();

    /// <summary>
    /// En qué estado se encuentra el mecanismo.
    /// </summary>
    /// <returns>Un enum representando el estado.</returns>
    Enum estado_actual();

    /// <summary>
    /// Cambia el estado del mecanismo.
    /// </summary>
    /// <param name="estado_nuevo">Nuevo estado al que poner el mecanismo.</param>
    void cambiar_estado(Enum estado_nuevo);
}
