/// <summary>
/// Interfaz para describir el comportamiento de mecanismos cuyo estado
/// está mejor descrito con un valor continuo (un float en este caso).
/// Este tipo de mecanismos suelen tener un límite máximo y mínimo, como
/// por ejemplo una placa de presión o un tirador.
/// </summary>
public interface IMecanismoContinuo : IMecanismo<float>
{
    /// <summary>Valor mínimo que puede alcanzar el «desplazamiento» del mecanismo.</summary>
    float min { get; set; }
    /// <summary>Valor máximo que puede alcanzar el «desplazamiento» del mecanismo.</summary>
    float max { get; set; }

    /// <summary>
    /// El estado continuo del mecanismo, que suele asociarse a algún tipo
    /// de desplazamiento. El valor debe encontrarse entre min y max.
    /// </summary>
    float desplazamiento { get; }
}
