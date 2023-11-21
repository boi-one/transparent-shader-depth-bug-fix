using System;
using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class FuzzyState : PathfindingState
{
    [SerializeField] private float fuzzyDuration = 2f;
    [SerializeField] private UnityEvent<float> onFuzzyTimerStart;
    [SerializeField] private UnityEvent onExitFuzzy;

    private bool _timerIsActive;
    private Coroutine _currentTimer;
    private VelocityRotator _velocityRotater;
    private Rotater2D _rotater2D;

    private float _startRotationSpeed;

    private void Awake()
    {
;        _velocityRotater = GetComponent<VelocityRotator>();
        _rotater2D = GetComponent<Rotater2D>();
        _startRotationSpeed = _rotater2D.Speed;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.PausePath = true;
        _rotater2D.Speed = _startRotationSpeed;
        _velocityRotater.IsEnabled = true;
        _rotater2D.IsEnabled = false;
        EnemyAI.Speed = 0;
        EnemyAI.forceBody.onCollisionEvent.AddListener(OnCollision);
    }

    public override void OnExit()
    {
        base.OnExit();
        _rotater2D.IsEnabled = false;
        _velocityRotater.IsEnabled = false;
        EnemyAI.forceBody.Remove(ForceTypes.Default);
        EnemyAI.forceBody.Remove(ForceTypes.Collision);
        EnemyAI.ResetSpeed();
        if(_currentTimer != null) StopCoroutine(_currentTimer);
        EnemyAI.forceBody.onCollisionEvent.RemoveListener(OnCollision);
    }

    public void SetGravity(bool disableGravity)
    {
        // todo: niet meer direct vanuit deze state de bool aanpassen, maar een method aanroepen van EnemyAI
        EnemyAI.movementForce.DisablesGravity = disableGravity;
    }
    public void OnCollision(CollisionForceEvent collisionEvent)
    {
        if (collisionEvent.type == CollisionEventTypes.Knockback) StopTimer();
        var currentSpeed = EnemyAI.forceBody.DesiredVelocity.sqrMagnitude;
        _rotater2D.Speed = _startRotationSpeed * Mathf.Min(1, currentSpeed);
        if (collisionEvent.isGroundHit && !_rotater2D.IsEnabled)
        {
            _rotater2D.IsEnabled = true;
            _velocityRotater.IsEnabled = false;
        }
        
        if (_timerIsActive || currentSpeed > 0.01f) return;
        
        StartTimer();
    }

    private void StopTimer()
    {
        if(_currentTimer != null) StopCoroutine(_currentTimer);
        _timerIsActive = false;
        _velocityRotater.IsEnabled = true;
        _rotater2D.IsEnabled = false;
    }

    private void StartTimer()
    {
        _timerIsActive = true;
        onFuzzyTimerStart?.Invoke(fuzzyDuration);
        _currentTimer = StartCoroutine(FuzzyDuration());
    }
    
    private IEnumerator FuzzyDuration()
    {
        yield return new WaitForSeconds(fuzzyDuration);
        _timerIsActive = false;
        onExitFuzzy?.Invoke();
    }
}