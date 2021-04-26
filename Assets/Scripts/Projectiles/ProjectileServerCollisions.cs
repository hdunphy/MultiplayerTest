using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileServerCollisions : IShootable, IDamageable
{
    private Projectile Projectile;

    private float Damage;
    private int Bounces;
    private bool isDestroyed = false;

    public override void NetworkStart()
    {
        if (!IsServer)
        {
            Destroy(this);
            //enabled = false;
        }
    }

    private void Start()
    {
        Projectile = GetComponent<Projectile>();
        Damage = Projectile.GetDamage();
        Bounces = Projectile.GetBounces();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(Damage);
            DespawnSelf();
        }
        else if (collider.CompareTag("Wall"))
        {
            if (--Bounces <= 0)
                DespawnSelf();
        }
    }

    private void DespawnSelf()
    {
        if (!isDestroyed && IsServer)
        {
            DetachParticlesClientRpc();
            isDestroyed = true;
            Destroy(gameObject, 0.01f);
        }
    }

    [ClientRpc]
    private void DetachParticlesClientRpc()
    {
        Projectile.DetachParticle();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Despawn");
    }
}
