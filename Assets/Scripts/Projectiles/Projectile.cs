using MLAPI;
using UnityEngine;

public class Projectile : NetworkBehaviour, IDamageable
{
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private float Damage;
    [SerializeField] private Rigidbody2D Rb;

    private bool isDestroyed = false;

    private void Start()
    {
        Rb.velocity = transform.right * ProjectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(Damage);
            DespawnSelf();
        }
    }

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
