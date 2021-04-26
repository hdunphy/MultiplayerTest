using MLAPI;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerClient : GameEventListener
{
    [SerializeField] NetworkObject NetworkObject;
    public override void RaiseEvent(ulong ClientId)
    {
        Debug.Log($"Client: {ClientId}, Owner: {NetworkObject.OwnerClientId}");
        if (NetworkObject.OwnerClientId == ClientId)
        {
            _unityEvent.Invoke(ClientId);
        }
    }
}
