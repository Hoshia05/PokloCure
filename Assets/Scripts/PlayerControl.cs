using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    private PlayerInput _inputActions;

    public Vector2 PlayerMovement;
    public Vector2 PlayerLineOfSight;

    public bool IsDashing;

    public UnityEvent PauseMenu;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        _inputActions = GetComponent<PlayerInput>();
        
        _inputActions.actions["Move"].performed += context => PlayerMovement = context.ReadValue<Vector2>();
        _inputActions.actions["Move"].canceled += context => PlayerMovement = Vector2.zero;

        _inputActions.actions["Dash"].performed += context => IsDashing = true;
        _inputActions.actions["Dash"].canceled += context => IsDashing = false;

        _inputActions.actions["PauseMenu"].performed += context => PauseMenu.Invoke();
    }

    private void Update()
    {
        UpdateLoS(_inputActions.actions["Look"].ReadValue<Vector2>());
    }

    void UpdateLoS(Vector2 inputmousePosition)
    {
        if(Instance != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(inputmousePosition.x, inputmousePosition.y, Camera.main.nearClipPlane));
            Vector2 characterPosition = transform.position;
            PlayerLineOfSight = (mousePosition - characterPosition).normalized;
        }

    }
}
