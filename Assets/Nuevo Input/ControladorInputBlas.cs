using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class ControladorInputBlas : MonoBehaviour
{
    public InputActionMap mapa_acciones;
    public Camera camara;

    [Range(1f, 30f)] // Sensibilidad de la camara
    public float sens = 15f;
    [Range(1f, 100f)] // Multiplicador de velocidad
    public float mulv = 10f;

    private void Update()
    {
        var vec_mov = mapa_acciones["moverse"].ReadValue<Vector2>() * Time.deltaTime * mulv;
        transform.Translate(vec_mov.x, 0, vec_mov.y);

        var vec_mir = mapa_acciones["mirar"].ReadValue<Vector2>() * Time.deltaTime * sens;
        transform.Rotate(new Vector3(0, vec_mir.x, 0), Space.Self);
        camara.transform.Rotate(new Vector3(-vec_mir.y, 0, 0));
    }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // pa este pues no
        //mapa_acciones["movimiento"].performed += CallbackMovimiento;
    }

    void OnEnable()
    {
        mapa_acciones.Enable();
    }

    void OnDisable()
    {
        mapa_acciones.Disable();
    }
}
