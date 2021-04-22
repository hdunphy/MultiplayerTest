using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] GameEvent _gameEvent;
    [SerializeField] UnityEvent<ulong> _unityEvent;

    void Awake() => _gameEvent.Register(this);
    void OnDestroy() => _gameEvent.Deregister(this);
    public void RaiseEvent(ulong ClientId) => _unityEvent.Invoke(ClientId);
}
