using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour, IDamageable
{
    private PlayerController PlayerController;

    public void TakeDamage(float damage)
    {
        PlayerController.TakeDamage(damage);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            enabled = false;
        }
        else
        {
            PlayerController = transform.root.GetComponentInChildren<PlayerController>();
        }
    }
}
