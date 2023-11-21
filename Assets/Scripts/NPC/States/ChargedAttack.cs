using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedAttack : AttackState
{
    protected override void Attack()
    {
        var targetPosition = base.GetCurrentTarget();
        var step = (targetPosition - transform.position).normalized * attackForce.Direction.magnitude;
        attackForce.Direction = step;
        EnemyAI.forceBody.Add(attackForce, new CallbackConfig()
        {
            OnFinish = MovementFinished
        });
    }
}
