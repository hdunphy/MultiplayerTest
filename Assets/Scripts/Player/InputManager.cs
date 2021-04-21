using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : NetworkBehaviour
{
    public static InputManager Instance;
    private PlayerController PlayerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Input manager already exists");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if(PlayerController != null)
        {
            PlayerController.OnDiedEvent -= PlayerDied;
        }
    }

    public void SetLocalPlayerController(PlayerController _PlayerController)
    {
        PlayerController = _PlayerController;
        _PlayerController.OnDiedEvent += PlayerDied;
    }

    private void PlayerDied()
    {
        PlayerController.OnDiedEvent -= PlayerDied;
        PlayerController = null;
    }

    public void OnMove(CallbackContext callback)
    {
        if (PlayerController != null)
            PlayerController.Move(callback.ReadValue<Vector2>());
    }

    public void OnLook(CallbackContext callback)
    {
        if (PlayerController != null)
            PlayerController.Look(callback.ReadValue<Vector2>());
    }

    public void OnFire(CallbackContext callback)
    {
        if (PlayerController != null)
            PlayerController.SetFiring(callback.performed);
    }

    public void OnDropMine(CallbackContext callback)
    {
        if (PlayerController != null)
            PlayerController.SetDropMine(callback.performed);
    }
}
