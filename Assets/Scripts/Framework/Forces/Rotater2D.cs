using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Rotater2D : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private RotatorDirection direction;
    [SerializeField] private bool isEnabled;
    [SerializeField] private bool autoReset;

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            if(AutoReset) ResetOrientation();
        }
    }

    public bool AutoReset
    {
        get => autoReset;
        set => autoReset = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public void SetDirection(CollisionForceEvent collisionForceEvent)
    {
        SetDirection(collisionForceEvent.newDirection);
    }
    
    public void SetDirection(Vector3 newDirection)
    {
        if (Mathf.Abs(newDirection.x) < 1f) return;
        
        direction = (RotatorDirection)(Mathf.Sign(newDirection.x) * -1);
    }
    
    private void ResetOrientation()
    {
        transform.rotation = Quaternion.identity;
    }

    private protected virtual void Update()
    {
        if (IsEnabled) Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(new Vector3( 0, 0, (int)direction) * Speed * Time.deltaTime);
    }
}
