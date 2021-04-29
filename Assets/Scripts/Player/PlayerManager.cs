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
        PlayerObject.SetActive(newValue);
        if (newValue)
        {
            PlayerController.Initialize();
        }
    }

    private void OnDestroy()
    {
        if (PlayerController != null)
        {
            PlayerController.OnDiedEvent -= PlayerController_OnDiedEvent;
        }
    }

    //Should only be run by the SERVER
    private void PlayerController_OnDiedEvent()
    {
        if (PlayerController.HasLivesLeft)
            StartCoroutine(InGamePlayerRespawn());


        IsPlayerActive.Value = false;
        if (!NetworkManager.Singleton.IsServer) Debug.LogWarning("Ran OnDiedEvent on server");
    }

    //Should only be run by the SERVER
    private IEnumerator InGamePlayerRespawn()
    {
        float _respawnTime = PlayerController._RespawnTime;
        yield return new WaitForSeconds(_respawnTime);

        Respawn();
    }

    public void Respawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Respawn Server");
            SetPlayerObjectActive(true);
        }
        else
        {
            Debug.Log("Respawn Client");
            SetPlayerObjectActiveServerRpc(true);
        }
    }

    [ServerRpc]
    private void SetPlayerObjectActiveServerRpc(bool _isActive)
    {
        SetPlayerObjectActive(_isActive);
    }

    private void SetPlayerObjectActive(bool _isActive)
    {
        if (_isActive)
        {
            PlayerController.SetTransformPosition(SpawnLocationManager.GetRandomSpawn());
        }
        IsPlayerActive.Value = _isActive;
    }
}
