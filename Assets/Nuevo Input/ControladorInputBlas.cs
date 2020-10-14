using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class ControladorInputBlas : MonoBehaviour
{
    public InputActionAsset mapa_acciones;
    public Camera camara;

    private CharacterController contr;

    [Range(1f, 30f)] // Sensibilidad de la camara
    public float sens = 15f;
    [Range(1f, 100f)] // Multiplicador de velocidad
    public float mulv = 10f;

    private void FixedUpdate()
    {
        var vec_mov = mapa_acciones["moverse"].ReadValue<Vector2>() * Time.fixedDeltaTime * mulv;
        contr.Move(new Vector3(vec_mov.x, -9.8f * Time.fixedDeltaTime, vec_mov.y));
        
    }

    void Awake()
    {
        contr = GetComponent<CharacterController>();

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
