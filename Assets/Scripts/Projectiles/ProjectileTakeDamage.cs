using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTakeDamage : MonoBehaviour, IDamageable
{
    private bool isDestroyed = false;

    private void DespawnSelf()
    {
        if (!isDestroyed)
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
