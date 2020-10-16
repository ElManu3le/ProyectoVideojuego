/// <summary>
/// Interfaz para describir el comportamiento de mecanismos cuyo estado
/// está mejor descrito con un valor continuo (un float en este caso).
/// Este tipo de mecanismos suelen tener un límite máximo y mínimo, como
/// por ejemplo una placa de presión o un tirador.
/// </summary>
public interface IMecanismoContinuo
{
    /// <summary>
    /// El estado continuo del mecanismo, que suele asociarse a algún tipo
    /// de desplazamiento.
    /// </summary>
    /// <returns>Un float entre el límite mínimo y máximo del mecanismo.</returns>
    float desplazamiento();

    /// <summary>
    /// Cambia el desplazamiento del mecanismo, manteniéndolo entre los
    /// límites mínimo y máximo.
    /// </summary>
    void cambiar_despl(float nuevo_despl);

    /// <summary>
    /// El desplazamiento del mecanismo, normalizado para comprenderlo entre
    /// los valores 0 y 1.
    /// </summary>
    /// <returns>Un float entre 0 y 1.</returns>
    float despl_normalizado();

    /// <summary>
    /// Los límites mínimo y máximo del mecanismo.
    /// </summary>
    /// <returns>
    /// Una tupla con los valores float mínimo y el máximo, en ese orden.
    /// </returns>
    (float min, float max) despl_min_max();
}
