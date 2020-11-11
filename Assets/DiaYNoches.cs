using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaYNoches : MonoBehaviour
{

    public float v = 10.5f;
    public Color a,b;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.right, v * Time.deltaTime);
        RenderSettings.ambientIntensity = Vector3.Angle(transform.forward, Vector3.up) / 180f;
    }
}
