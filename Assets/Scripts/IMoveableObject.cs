using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableObject
{
    float GetMoveSpeed();
    Vector2 GetNetworkPosition();

    public event Action<Vector2> UpdateVelocityDirection;

    void SetNetworkPosition(Vector2 position);
}
