using System;
using UnityEngine;
using UnityEngine.Events;

public class LockedState : StateBehaviour, ILockable
{
    private EnemyAI _enemyAI;
    private TargetRotator _rotator;
    private CharacterOrientation _startOrientation;
    private ForceBody _forceBody;

    private UnityEvent _onLockRelease = new UnityEvent();
    
    public EnemyAI EnemyAI => _enemyAI ??= GetComponent<EnemyAI>();
    public TargetRotator Rotator => _rotator ??= GetComponent<TargetRotator>();
    

    private void FixedUpdate()
    {
        Rotator.Target = RotationTarget;
    }

    public override void OnEnter()
    {
        EnemyAI.ResetMovement();
        EnemyAI.CurrentTarget = transform.position;
        EnemyAI.PausePath = false;
        _startOrientation = Rotator.CurrentOrientation;
        Rotator.CurrentOrientation = CharacterOrientation.Forward;
        Rotator.IsEnabled = true;
    }
    public override void OnExit()
    {
        base.OnExit();
        Rotator.Reset();
        Rotator.CurrentOrientation = _startOrientation;
        _onLockRelease.Invoke();
    }
    public Transform Transform => transform;
    public ForceBody ForceBody => _forceBody ??= GetComponent<ForceBody>();
    public Transform RotationTarget { get; set; }
    public UnityEvent OnLockRelease => _onLockRelease;

    public void StateTrigger(string triggerName, bool value)
    {
        StateMachine.SetTrigger(triggerName, value);
    }
}
