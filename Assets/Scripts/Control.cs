using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Control : MonoBehaviour
{

    public float lateralmove;
    public float verticalmove;
    public int disparar;
    public CharacterController soldado;
    private Animator animador;

    public float velocidad=1; 
    // Start is called before the first frame update
    void Start()
    {
        soldado = GetComponent<CharacterController>();
        animador = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

        lateralmove = Input.GetAxis("Horizontal");
        verticalmove = Input.GetAxis("Vertical");
        //disparar = Input.GetKey("Fire1")
        
    }

    private void FixedUpdate()
    {


        if ( verticalmove > 0)
            animador.SetBool("Andar", true);
        else animador.SetBool("Andar", false);

        if (verticalmove < 0)
            animador.SetBool("Retroceder", true);
        else animador.SetBool("Retroceder", false);

        if (lateralmove > 0)
            animador.SetBool("Derecha", true);
        else animador.SetBool("Derecha", false);

        if (lateralmove < 0)
            animador.SetBool("Izquierda", true);
        else animador.SetBool("Izquierda", false);

        if ((lateralmove) + (verticalmove) > 0)
            animador.SetBool("Derecha", true);
        else animador.SetBool("Derecha", false);

        if ((lateralmove) + (verticalmove) < 0)
            animador.SetBool("Izquierda", true);
        else animador.SetBool("Izquierda", false);


        







        soldado.Move(new Vector3(lateralmove, 0, verticalmove) * velocidad * Time.deltaTime);
    }
}
