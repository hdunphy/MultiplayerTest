using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMovement : NetworkBehaviour
{

    private float MoveSpeed;
    private IMoveableObject Controller;
    private Vector2 nextPosition;
    private float distance, t;


    public override void NetworkStart()
    {
        if (IsServer)
        {
            enabled = false;
        }
        else
        {
            Controller = GetComponent<IMoveableObject>();
            MoveSpeed = Controller.GetMoveSpeed();
            transform.position = Controller.GetNetworkPosition();
        }
    }

    private void FixedUpdate()
    {
        nextPosition = Controller.GetNetworkPosition();
        distance = Vector2.Distance(transform.position, nextPosition);
        if (distance > 0.01f)
        {
            t = distance * (1 / MoveSpeed);
            transform.position = Vector2.Lerp(transform.position, nextPosition, t);
        }
    }
}
