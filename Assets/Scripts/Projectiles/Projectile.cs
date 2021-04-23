using MLAPI;
using MLAPI.NetworkVariable;
using System;
using System.IO;
using UnityEngine;

public class Projectile : IShootable
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float Damage;
    [SerializeField, Range(0, 4)] private int Bounces;
    [SerializeField] private Rigidbody2D Rb;

    private Vector2 velocity;

    private void Start()
    {
        Rb.velocity = transform.right * ProjectileSpeed;
        velocity = Rb.velocity;
    }

    private void Update()
    {
        if(Rb.velocity != velocity)
        {
            velocity = Rb.velocity;
            transform.right = velocity;
        }
    }

    public float GetDamage() => Damage;
    public int GetBounces() => Bounces;
}
