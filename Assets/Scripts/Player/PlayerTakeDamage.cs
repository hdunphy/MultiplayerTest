using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour, IDamageable
{
    [SerializeField] PlayerController _controller;

    public void TakeDamage(float damage)
    {
        _controller.TakeDamage(damage);
    }
}
