using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IShootable : NetworkBehaviour
{
    protected PlayerFiringController ShootController;

    public void Initialize(PlayerFiringController _playerShootController)
    {
        ShootController = _playerShootController;
        ShootController.AddPrefab();
    }

    private void OnDestroy()
    {
        if(IsServer)
            ShootController.RemovePrefab();
    }
}
