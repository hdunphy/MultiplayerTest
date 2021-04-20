using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class ProjectileServerCollisions : NetworkBehaviour, IDamageable
{
    private float Damage;
    private int Bounces;
    private bool isDestroyed = false;

    public override void NetworkStart()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        Damage = GetComponent<Projectile>().GetDamage();
        Bounces = GetComponent<Projectile>().GetBounces();
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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Wall"))
    //    {
    //        if (--Bounces <= 0)
    //            DespawnSelf();
    //    }
    //}

    private void DespawnSelf()
    {
        if (!isDestroyed && IsServer)
        {
            isDestroyed = true;
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    public void TakeDamage(float damage)
    {
        DespawnSelf();
    }
}
