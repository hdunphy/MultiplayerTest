using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Server only
public class PlayerFiringController : NetworkBehaviour
{
    [SerializeField, Min(1)] int MaxPrefabsInScene;
    [SerializeField] float FireRate;
    [SerializeField] Transform FirePoint;
    [SerializeField, Tooltip("Must have IShootable Component")] IShootable PrefabToShoot;

    public bool IsFiring { get; private set; }

    private int CurrentPrefabCount;
    private float m_LastShot;

    public override void NetworkStart()
    {
        if(!IsServer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsFiring && (CurrentPrefabCount < MaxPrefabsInScene) && ((Time.time - m_LastShot) > FireRate))
        {
            m_LastShot = Time.time;

            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        var _prefab = Instantiate(PrefabToShoot, FirePoint.position, FirePoint.rotation);
        _prefab.Initialize(this);
        _prefab.GetComponent<NetworkObject>().Spawn();
    }

    public void SetIsFiring(bool _isFiring) => IsFiring = _isFiring;
    public void AddPrefab() => CurrentPrefabCount++;
    public void RemovePrefab() => CurrentPrefabCount--;

    //[ServerRpc]
    //private void SpawnPrefabServerRpc(ServerRpcParams rpcParams = default)
    //{
    //    var _prefab = Instantiate(PrefabToShoot, FirePoint.position, FirePoint.rotation);
    //    _prefab.GetComponent<NetworkObject>().Spawn();
    //}
}
