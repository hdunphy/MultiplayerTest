using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineClientController : NetworkBehaviour
{
    [SerializeField] private GameObject Explosion;
    [SerializeField] private SpriteRenderer MineRenderer;

    [ClientRpc]
    public void ExplodeParticleEffectClientRpc()
    {
        Explosion.SetActive(true);
        MineRenderer.enabled = false;
    }
}
