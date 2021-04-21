using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineServerController : NetworkBehaviour
{
    [SerializeField] private float TimerDelay;
    [SerializeField] private float ExplosionRadius;
    [SerializeField] private float ExplosionInnerRadius;
    [SerializeField] private float Damage;


    private MineClientController MineClient;
    private float ExplosionTime;

    public override void NetworkStart()
    {
        if (!IsServer) Destroy(this);
    }

    private void Start()
    {
        MineClient = GetComponent<MineClientController>();
        ExplosionTime = TimerDelay + Time.time;
    }

    private void Update()
    {
        if(Time.time >= ExplosionTime)
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        MineClient.ExplodeParticleEffectClientRpc();

        DealDamage(ExplosionInnerRadius);

        yield return new WaitForSeconds(0.25f);

        DealDamage(ExplosionRadius);
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void DealDamage(float _radius)
    {
        var colliderInRadius = Physics2D.OverlapCircleAll(transform.position, _radius);

        foreach (Collider2D _collider in colliderInRadius)
        {
            if (_collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage);
            }
        }
    }
}
