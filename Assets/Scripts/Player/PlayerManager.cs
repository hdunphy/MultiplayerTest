using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public GameObject PlayerObject;
    public NetworkVariableBool IsPlayerActive { get; } = new NetworkVariableBool(true);
    private PlayerController PlayerController;

    private void Start()
    {
        PlayerController = PlayerObject.GetComponent<PlayerController>();
        PlayerController.OnDiedEvent += PlayerController_OnDiedEvent;
        IsPlayerActive.OnValueChanged += IsPlayerActiveValueChanged;

        PlayerObject.SetActive(IsPlayerActive.Value);
    }

    private void IsPlayerActiveValueChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"Changed Player Active Value {newValue}");
        PlayerObject.SetActive(newValue);
        if (newValue)
        {
            PlayerController.Initialize();
        }
    }

    private void OnDestroy()
    {
        if(PlayerController != null)
        {
            PlayerController.OnDiedEvent -= PlayerController_OnDiedEvent;
        }
    }

    //Should only be run by the SERVER
    private void PlayerController_OnDiedEvent()
    {
        IsPlayerActive.Value = false;
        if (!NetworkManager.Singleton.IsServer) Debug.LogWarning("Ran OnDiedEvent on server");
    }

    public void Respawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Respawn Server");
            IsPlayerActive.Value = true;
        }
        else
        {
            Debug.Log("Respawn Client");
            SetPlayerObjectActiveServerRpc(true);
        }
        //SetPlayerObjectActiveServerRpc(true);
    }

    [ServerRpc]
    private void SetPlayerObjectActiveServerRpc(bool _isActive)
    {
        IsPlayerActive.Value = _isActive;
        //SetPlayerObjectActiveClientRpc(_isActive);
    }

    [ClientRpc]
    private void SetPlayerObjectActiveClientRpc(bool _isActive)
    {
        PlayerObject.SetActive(_isActive);
        if(_isActive)
            PlayerObject.GetComponent<PlayerController>().Initialize();
    }
}
