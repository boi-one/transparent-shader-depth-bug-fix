using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class ObjectFlipper : MonoBehaviour
{
    private CollisionEventTypes _currentCollisionState;
    private const float VelocityTolerance = 0.5f;
    [SerializeField]private ForceBody forceBody;

    public bool IsEnabled { get; set; }

    private readonly Dictionary<CollisionEventTypes, int[]> _xForceRotation = new Dictionary<CollisionEventTypes, int[]>()
    {
        {CollisionEventTypes.Knockback, new int[] { 1, -1 }},
        {CollisionEventTypes.Bounce, new int[] { -1, 1 }}
    };

    private void Update()
    {
        if (IsEnabled) SetXDirection(forceBody.Velocity);
    }

    private void SetXDirection(Vector3 velocity)
    {
        if (_xForceRotation.TryGetValue(_currentCollisionState, out int[] value))
        {
            int xScale = 1;

            if (velocity.x < -VelocityTolerance) xScale = value[0];
            if (velocity.x > VelocityTolerance) xScale = value[1];
            
            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        }
    }

    public void SetCollisionState(CollisionForceEvent collisionForceEvent)
    {
        _currentCollisionState = collisionForceEvent.type;
    }
}
