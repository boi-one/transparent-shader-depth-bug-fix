using System.Collections.Generic;
using UnityEngine;

public class TargetRotator : VelocityRotator
{
    
    [SerializeField] private Transform target;
    
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    public void UnTarget()
    {
        Target = null;
    }

    public override Vector3 CurrentDirection => Target != null ? Target.position - transform.position : base.CurrentDirection;

}