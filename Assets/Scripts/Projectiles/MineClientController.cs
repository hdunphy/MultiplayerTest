using MLAPI;
using MLAPI.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineClientController : NetworkBehaviour
{
    [SerializeField] private GameObject Explosion;
    [SerializeField] private SpriteRenderer MineRenderer;

    [SerializeField] private Color FlashColor;
    [SerializeField] private float FlashTime;

    private float TimerDelay;

    private void Start()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        float interval = (TimerDelay - (4 * FlashTime)) / 4;
        Color startColor = MineRenderer.color;

        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(interval);
            MineRenderer.color = FlashColor;
            yield return new WaitForSeconds(FlashTime);
            MineRenderer.color = startColor;
        }

        while (MineRenderer.enabled)
        {
            yield return new WaitForSeconds(FlashTime);
            MineRenderer.color = FlashColor;
            yield return new WaitForSeconds(FlashTime);
            MineRenderer.color = startColor;
        }
    }

    [ClientRpc]
    public void ExplodeParticleEffectClientRpc()
    {
        Explosion.SetActive(true);
        MineRenderer.enabled = false;
    }

    public void SetTimer(float timerDelay)
    {
        TimerDelay = timerDelay;
    }
}
