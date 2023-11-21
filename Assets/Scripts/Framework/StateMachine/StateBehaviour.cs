using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StateMachine.StateMachine))]
public abstract class StateBehaviour : MonoBehaviour
{
    private StateMachine.StateMachine _stateMachine;
    [SerializeField] private UnityEvent onEnter = new UnityEvent();
    [SerializeField] private UnityEvent onExit = new UnityEvent();

    public virtual void OnEnter()
    {
        onEnter?.Invoke();
    }

    public virtual void OnExit()
    {
        onExit?.Invoke();
    }

    public StateMachine.StateMachine StateMachine
    {
        get
        {
            if (_stateMachine == null) _stateMachine = GetComponent<StateMachine.StateMachine>();
            return _stateMachine;
        }
    }
}
