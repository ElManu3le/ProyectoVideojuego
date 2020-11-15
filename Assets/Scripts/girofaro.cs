using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class girofaro : MonoBehaviour
{

    private GameObject foco;

    private float giro = 20f;
    // Start is called before the first frame update
    void Start()
    {
        foco = GameObject.Find("linterna");
        
    }

    // Update is called once per frame
    void Update()
    {

        foco.transform.Rotate(new Vector3(0f, giro * Time.deltaTime, 0f), Space.World);
        
    }
}
