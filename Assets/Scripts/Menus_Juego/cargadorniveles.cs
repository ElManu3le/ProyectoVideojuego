using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cargadorniveles : MonoBehaviour
{
    void OnGUI()
    {
        //This displays a Button on the screen at position (20,30), width 150 and height 50. The button’s text reads the last parameter. Press this for the SceneManager to load the Scene.
        if (GUI.Button(new Rect(20, 30, 150, 30), "Nueva Partida"))
        {
            SceneManager.LoadScene("TestMovimientoBlas", LoadSceneMode.Single);
        }

        //Whereas pressing this Button loads the Additive Scene.
        if (GUI.Button(new Rect(20, 90, 150, 30), "Cargar Partida"))
        {
            SceneManager.LoadScene("seleccionador", LoadSceneMode.Single);
            
        }

        if (GUI.Button(new Rect(20, 150, 150, 30), "Salir juego"))
        {
           
        }
    }
}
