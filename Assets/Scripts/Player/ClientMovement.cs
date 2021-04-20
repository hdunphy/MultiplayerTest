using MLAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMovement : NetworkBehaviour
{

    private float MoveSpeed;
    private IMoveableObject Controller;
    private Vector2 nextPosition;


    public override void NetworkStart()
    {
        if(IsServer)
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
        float distance = Vector2.Distance(transform.position, nextPosition);
        float t = distance * (1 / MoveSpeed);
        transform.position = Vector2.LerpUnclamped(transform.position, nextPosition, t);
    }
}
