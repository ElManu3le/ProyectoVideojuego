using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Misc
{
    /// <summary>
    /// Devuelve una máscara de capa para ignorar todas las capas
    /// pasadas como parámetros.
    /// </summary>
    /// <param name="capas">Capas a ignorar por la máscara</param>
    /// <returns></returns>
    public static LayerMask ignorar_capas(params LayerMask[] capas)
    {
        LayerMask combinacion = 0;
        foreach (var capa in capas)
            combinacion |= capa;
        return ~combinacion;
    }
}
