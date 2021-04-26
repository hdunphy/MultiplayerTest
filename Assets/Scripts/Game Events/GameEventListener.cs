using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] GameEvent _gameEvent;
    [SerializeField] protected UnityEvent<ulong> _unityEvent;

    void Awake() => _gameEvent.Register(this);
    void OnDestroy() => _gameEvent.Deregister(this);
    public virtual void RaiseEvent(ulong ClientId) => _unityEvent.Invoke(ClientId);
}
