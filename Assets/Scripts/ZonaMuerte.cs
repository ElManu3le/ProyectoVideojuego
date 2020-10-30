using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaMuerte : MonoBlashaviour
{
    public static int vidas_restantes = 3;

    private AudioSource audio;
    public Transform spawn;

    private void Start()
    {
        audio = Blas.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
       
        Debug.Log("Muerte! A!");
        if (other.gameObject.layer == 10)
        {
            
            if (--vidas_restantes <= 0)
            {
                vidas_restantes = 3;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene("menu_inicio");   
            }
            else
            {
                audio.Play();
                StartCoroutine(moricion());
            }
        }
    }

    private IEnumerator moricion()
    {
        yield return new WaitForSeconds(.38f);
        Blas.transform.position = spawn.position;
    }
}
