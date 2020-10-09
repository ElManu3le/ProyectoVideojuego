using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacaDePresion : MonoBehaviour
{
    private Vector3 posabajo;
    private Vector3 posarriba;
    private GameObject placa;
    private float desparriba = .1f;

    [Range(.1f, 100f)]
    public float masaNecesariaParaActivar = 50f;
    public float estado { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        posarriba = transform.parent.position;
        posabajo = posarriba + Vector3.down * desparriba;
        placa = transform.parent.gameObject;
        estado = 0f;
    }

    private int frames_pisando = 0;
    private int frames_nec = 20;
    private bool dentro = false;

    private void Update()
    {
        if (dentro && frames_pisando < frames_nec) frames_pisando++;
        else if (frames_pisando > 0) frames_pisando--;

        estado = Mathf.Clamp((69f / masaNecesariaParaActivar) * (float)frames_pisando / (float)frames_nec, 0, 1);
        placa.transform.position = Vector3.Lerp(posarriba, posabajo, estado);
        Debug.Log("Placa[0,1]: " + estado);
    }

    private void OnTriggerEnter(Collider other)
    {
        dentro = true;
    }

    private void OnTriggerExit(Collider other)
    {
        dentro = false;
    }
}
