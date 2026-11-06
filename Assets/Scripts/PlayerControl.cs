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
    public Vector2 MousePosition;

    public bool IsDashing;
    public bool IsDrunk;

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
            return;
        }


        _inputActions = GetComponent<PlayerInput>();
        
        // Move / Dash 는 이벤트로 값을 캐시하지 않고 매 프레임 직접 읽는다.
        // (캐시 방식은 canceled 이벤트를 놓치면 입력이 계속 눌린 채로 남는다 — 에디터 포커스 이동 등)

        _inputActions.actions["PauseMenu"].performed += context => PauseMenu.Invoke();


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _inputActions.actions["DebugButton"].performed += context => DebugButton();
#endif
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        PlayerMovement = _inputActions.actions["Move"].ReadValue<Vector2>();
        IsDashing = _inputActions.actions["Dash"].IsPressed();

        UpdateLoS(_inputActions.actions["Look"].ReadValue<Vector2>());
    }

    void UpdateLoS(Vector2 inputmousePosition)
    {
        if(Instance != null)
        {
            MousePosition = Camera.main.ScreenToWorldPoint(new Vector3(inputmousePosition.x, inputmousePosition.y, Camera.main.nearClipPlane));
            Vector2 characterPosition = transform.position;
            PlayerLineOfSight = (MousePosition - characterPosition).normalized;
        }

    }

    void DebugButton()
    {
        Debug.Log("Debug Button");

        PlayerScript.Instance.IncreaseWeaponSlot();
    }

    public Vector2 GetPlayerMovement()
    {
        if (IsDrunk)
        {
            return new Vector2(-PlayerMovement.x, -PlayerMovement.y);
        }
        else
        {
            return PlayerMovement;
        }
    }
}
