/// <summary>
/// Interfaz para describir el comportamiento de mecanismos con dos
/// estados diferentes (que no tienen que ser los únicos): activo y
/// no activo. Por ejemplo, un botón o un(a palanca de un) cambio de
/// vías.
/// </summary>
public interface IMecanismoActivable
{
    /// <summary>
    /// Si el mecanismo se puede considerar activado o no.
    /// </summary>
    /// <returns>True si está activado, false si no.</returns>
    bool esta_activado();

    /// <summary>
    /// Activa el mecanismo, o no hace nada si estaba ya activo.
    /// </summary>
    void activar();

    /// <summary>
    /// Desactiva el mecanismo, o no hace nada si estaba ya activo.
    /// </summary>
    void desactivar();
}
