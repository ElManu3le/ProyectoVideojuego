using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        

        
        if (GUI.Button(new Rect(20, 90, 150, 30), "Atras"))
        {
            SceneManager.LoadScene("menu_inicio");

        }
    }

  

    public void CargarJuego(string nombremenu)
    {
        SceneManager.LoadScene(nombremenu);
        
    }
}
