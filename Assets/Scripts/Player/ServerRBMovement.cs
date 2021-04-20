using MLAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRBMovement : NetworkBehaviour
{
    private Rigidbody2D Rb;
    private IMoveableObject Controller;

    private float MoveSpeed;

    public override void NetworkStart()
    {
        Rb = GetComponent<Rigidbody2D>();
        if (!IsServer)
        {
            enabled = false;
            Destroy(Rb);
        }
        else
        {
            Controller = GetComponent<IMoveableObject>();
            Controller.UpdateVelocityDirection += OnClientChangeDirection;
            MoveSpeed = Controller.GetMoveSpeed();
        }
    }

    private void FixedUpdate()
    {
        Controller.SetNetworkPosition(transform.position);
    }

    private void OnClientChangeDirection(Vector2 _direction)
    {
        Rb.velocity = _direction.normalized * MoveSpeed;
    }

    private void OnDestroy()
    {
        if (IsServer) { Controller.UpdateVelocityDirection -= OnClientChangeDirection; }
    }
}
