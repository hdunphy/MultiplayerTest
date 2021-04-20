using MLAPI;
using MLAPI.NetworkVariable;
using System;
using System.IO;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float Damage;
    [SerializeField, Range(0, 4)] private int Bounces;
    [SerializeField] private Rigidbody2D Rb;
    //public NetworkVariableVector2 NetworkPosition { get; } = new NetworkVariableVector2();

    //public event Action<Vector2> UpdateVelocityDirection;

    private void Start()
    {
        //UpdateVelocityDirection?.Invoke(transform.right);
        //NetworkPosition.Value = transform.position;
        Rb.velocity = transform.right * ProjectileSpeed;
    }

    //public float GetMoveSpeed() => ProjectileSpeed;

    //public Vector2 GetNetworkPosition() => NetworkPosition.Value;

    //public void SetNetworkPosition(Vector2 _position) => NetworkPosition.Value = _position;

    public float GetDamage() => Damage;
    public int GetBounces() => Bounces;
}
