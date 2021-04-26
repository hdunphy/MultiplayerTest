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
    [SerializeField] private ParticleSystem ParticleSmoke;

    private Vector2 velocity;

    private void Start()
    {
        Rb.velocity = transform.right * ProjectileSpeed;
        velocity = Rb.velocity;

        ParticleSmoke.Play();
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

    public void DetachParticle()
    {
        //var main = ParticleSmoke.main;
        //main.loop = false;
        var emissions = ParticleSmoke.emission;
        emissions.rateOverTime = 0;
        ParticleSmoke.transform.parent = null;
        Destroy(ParticleSmoke.gameObject, 2f);
    }
}
