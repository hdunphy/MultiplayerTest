using MLAPI;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerClient : GameEventListener
{
    [SerializeField] NetworkBehaviour NetworkBehavior;
    public override void RaiseEvent(ulong ClientId)
    {
        //Debug.Log($"GameObject {gameObject.name}. Client: {ClientId}, Owner: {NetworkBehavior.OwnerClientId}");
        if (NetworkBehavior.OwnerClientId == ClientId)
        {
            _unityEvent.Invoke(ClientId);
        }
    }
}
