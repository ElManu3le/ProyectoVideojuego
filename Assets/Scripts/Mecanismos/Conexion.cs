using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conexion : MonoBehaviour
{
    [Range(0f, 1f)]
    public float pctj = 0f;
    public Color apagado = Color.gray,
                 encendido = Color.green;

    private Material mat;
    private static Shader std, unl;

    private bool init = false;

    private void Awake()
    {
        if (!init)
        {
            std = Shader.Find("Standard");
            unl = Shader.Find("Unlit/Color");
            init = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mat = new Material(std)
        {
            color = apagado
        };
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    // Update is called once per frame
    void Update()
    {
        mat.color = Color.Lerp(apagado, encendido, pctj);
        if (pctj > .99f) mat.shader = unl;
        else mat.shader = std;
    }
}
