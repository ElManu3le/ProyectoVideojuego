using UnityEngine;

public static class Mates
{
    /// <summary>
    /// https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
    /// (Segunda respuesta)
    /// </summary>
    /// <param name="punto">Punto cuya distancia al segmento queremos saber</param>
    /// <param name="p_seg_1">Punto inicial del segmento</param>
    /// <param name="p_seg_2">Punto final del segmento</param>
    /// <returns>Distancia entre el punto punto y el segmento de extremos p_seg_1 y p_seg_2</returns>
    public static Vector3 PuntoMasCercanoASegmento(Vector3 punto, Vector3 p_seg_1, Vector3 p_seg_2)
    {
        Vector3 direccion_segm = p_seg_2 - p_seg_1;
        float magnitud_segm = direccion_segm.magnitude;
        direccion_segm.Normalize();
        float longitud_proyectada = Mathf.Clamp(Vector3.Dot(punto - p_seg_1, direccion_segm), 0f, magnitud_segm);
        return p_seg_1 + direccion_segm * longitud_proyectada;
    }
}
