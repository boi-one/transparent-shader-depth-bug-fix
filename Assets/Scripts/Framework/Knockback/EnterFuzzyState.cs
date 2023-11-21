using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterFuzzyState : MonoBehaviour
{
    [SerializeField] private string fuzzyStateTriggerName = "isHit";
    private StateMachine.StateMachine _stateMachine;
    
    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine.StateMachine>();
    }

    public void Execute(CollisionForceEvent collisionForceEvent)
    {
        if (collisionForceEvent.type == CollisionEventTypes.Bounce)
        {
            _stateMachine.SetTrigger(fuzzyStateTriggerName, true);
        }
    }
}
