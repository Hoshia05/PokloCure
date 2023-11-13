using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    private PlayerInput _inputActions;

    public Vector2 PlayerMovement;
    public Vector2 PlayerLineOfSight;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        _inputActions = GetComponent<PlayerInput>();
        
        _inputActions.actions["Move"].performed += context => PlayerMovement = context.ReadValue<Vector2>();
        _inputActions.actions["Move"].canceled += context => PlayerMovement = Vector2.zero;

        _inputActions.actions["Look"].performed += context => UpdateLoS(context.ReadValue<Vector2>());

    }

    // Update is called once per frame
    private void Update()
    {
        //UpdateLoS();
    }


    void UpdateLoS(Vector2 inputmousePosition)
    {
        //Vector2 inputmousePosition = Mouse.current.position.ReadValue();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(inputmousePosition.x, inputmousePosition.y, Camera.main.nearClipPlane));

        Vector2 characterPosition = transform.position;

        PlayerLineOfSight = (mousePosition - characterPosition).normalized;
    }
}
