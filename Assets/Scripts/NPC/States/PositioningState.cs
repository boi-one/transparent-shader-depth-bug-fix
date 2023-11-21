using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositioningState : PathfindingState
{
    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float attackDistance = 10f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 offset;
    
    [SerializeField] private UnityEvent onInRange = new UnityEvent();
    [SerializeField] private UnityEvent onOutofRange = new UnityEvent();
    
    private TargetRotator _targetRotator;
    
    [SerializeField] private Force repositionForce;
    
    private bool _hasWaited;
    private void Awake()
    {
        _targetRotator = GetComponent<TargetRotator>();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _hasWaited = false;
        EnemyAI.PausePath = true;
        EnemyAI.ResetMovement();

        FindNearestPlayer(maxDistance);
        if (CurrentPlayer == null)
        {
            SetTrigger("outofRange");
            return;
        }
        _targetRotator.Target = CurrentPlayer.transform;
        Reposition();
    }

    private void Reposition()
    {
        var targetPosition = GetCurrentTarget();
        var direction = targetPosition - transform.position;
        repositionForce.Direction = direction;
        repositionForce.Duration = direction.magnitude / speed;

        if(direction.magnitude > 0.3f && !_hasWaited)
        {
            _hasWaited = true;
            StartCoroutine(Wait());
            return;
        }
        AddForce();
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.3f);
        Reposition();
        
    }

    private void AddForce()
    {
        EnemyAI.forceBody.Add(repositionForce, new CallbackConfig()
        {
            OnFinish = OnPosition,
            OnCancel = OnPositioningCancelled
        });
    }

    private void OnPositioningCancelled()
    {
        SetTrigger("cancelled");
    }

    public override void OnExit()
    {
        _targetRotator.Reset();
        _targetRotator.UnTarget();
        base.OnExit();
    }
    

    private void OnPosition()
    {
        FindNearestPlayer(maxDistance);
        if (CurrentPlayer == null)
        {
            SetTrigger("outofRange");
            onOutofRange?.Invoke();
            return;
        }
        var distance = (CurrentPlayer.transform.position - transform.position).magnitude;

        if (distance > maxDistance)
        {
            SetTrigger("outofRange");
            onOutofRange?.Invoke();
            return;
        }
        if (distance < attackRange)
        {
            SetTrigger("inRange");
            onInRange?.Invoke();
            return;
        }
        Reposition();
    }
    
    public override Vector3 GetCurrentTarget()
    {
        if (GameObject.FindWithTag("Player") == null) return Vector3.zero;

        FindNearestPlayer(maxDistance);

        if (CurrentPlayer == null) return Vector3.zero;

        var currentPosition = transform.position;
        var playerPosition = CurrentPlayer.transform.position + offset;
        
        var position = playerPosition;
        
        position.x = currentPosition.x;

        var attackPosition = playerPosition + (position - playerPosition).normalized * attackDistance;
        
        return attackPosition;
    }




}
