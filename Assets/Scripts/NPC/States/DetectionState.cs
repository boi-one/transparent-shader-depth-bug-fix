using System;
using UnityEngine;
using UnityEngine.Events;

public class DetectionState : PathfindingState
{
    
    [SerializeField] private float detectionTime = 3f;
    [SerializeField] private float inRangeDistance = 10f;
    [SerializeField] private UnityEvent onOutofRange = new UnityEvent();
    [SerializeField] private UnityEvent onDetectedPlayer = new UnityEvent();
    
    private TargetRotator _targetRotator;
    
    private void Awake()
    {
        _targetRotator = GetComponent<TargetRotator>();
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.PausePath = true;
        Invoke("PlayerDetected", detectionTime);
        FindNearestPlayer();
        LookAtTarget();
        EnemyAI.ResetMovement();
    }

    private void LookAtTarget()
    {
        if (!_targetRotator) return;
        _targetRotator.Target = CurrentPlayer.transform;
    }

    public override void OnExit()
    {
        CancelInvoke();
        ResetRotation();
    }

    private void ResetRotation()
    {
        if (!_targetRotator) return;
        _targetRotator.Reset();
        _targetRotator.UnTarget();
    }

    private void PlayerDetected()
    {
        if (CurrentPlayer == null)
        {
            SetTrigger("outofRange");
            return;
        }
            
        var distance = (CurrentPlayer.transform.position - transform.position).magnitude;
        
        string status;
        if (distance > inRangeDistance)
        {
            status =  "outofRange";
            onOutofRange?.Invoke();
        }
        else
        {
            status =  "detectedPlayer";
            onDetectedPlayer?.Invoke();
        }
        
        SetTrigger(status);
    }

}
