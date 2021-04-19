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

    public void SetLocalPlayerController(PlayerController _PlayerController)
    {
        PlayerController = _PlayerController;
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

    public void RemoveController(PlayerController _playerController)
    {
        if (_playerController == PlayerController)
            PlayerController = null;
    }
}
