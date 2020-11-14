using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaYNoches : MonoBehaviour
{

    public float v = 10.5f;
    public Color a,b;

    public Light sol, luna;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.right, v * Time.deltaTime);
        
        var e = Mathf.Clamp(Vector3.Angle(transform.forward, Vector3.up), 60, 180);
        sol.intensity = (e - 60f) / 120f; // numeros magicos numeros magicos

        RenderSettings.ambientLight = Color.Lerp(a, b, sol.intensity);
        RenderSettings.reflectionIntensity = RenderSettings.ambientIntensity = sol.intensity;

        luna.intensity = .05f - sol.intensity;
    }
}
