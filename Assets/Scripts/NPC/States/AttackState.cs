using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

public abstract class AttackState : PathfindingState
{
    [SerializeField] private float damageAmount = 25f;

    private GameObject _playerObject;
    private HitSystem _hitSystem;
    private EntityStats _entityStats;

    [SerializeField] private float maxDistance = 25f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 hitAreaOffset = new Vector3(1, 0, 0);
    [SerializeField] private int direction = 1;

    [SerializeField] protected Force attackForce;
    [SerializeField] private UnityEvent onAttackCompleted = new UnityEvent();
    
    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.PausePath = true;
        EnemyAI.ResetMovement();
        _hitSystem = GetComponent<HitSystem>();
        _entityStats = GetComponent<EntityStats>();
        Attack();
        _hitSystem.IsPerformingAttack = true;
        _hitSystem.CurrentHitboxNumber = 0;
    }

    public override void OnExit()
    {
        _hitSystem.IsPerformingAttack = false;
    }

    private void FixedUpdate()
    {
        foreach (GameObject target in GetTargetsInRange())
        {
            if (_hitSystem.CheckHasHit(target)) continue;
            AddHit(target);
        }
    }

    protected virtual void Attack()
    {
        //base attack?
    }

    private protected void MovementFinished()
    {
        onAttackCompleted?.Invoke();
        SetTrigger("detectedPlayer");
    }

    public override Vector3 GetCurrentTarget()
    {
        if (GameObject.FindWithTag("Player") == null) return Vector3.zero;

        _playerObject = EnemyAI.GetClosestTarget(maxDistance);

        if (_playerObject == null) return Vector3.zero;

        var currentPosition = transform.position;
        var playerPosition = _playerObject.transform.position + offset;

        var step = playerPosition - currentPosition;
        var position = playerPosition;

        if (Mathf.Abs(step.x) < Mathf.Abs(step.y)) position.x = currentPosition.x;
        else position.y = currentPosition.y;
        
        return position;
    }

    private List<GameObject> GetTargetsInRange()
    {

        var offset = hitAreaOffset;
        offset.x *= direction;

        var hitBounds = _hitSystem.CurrentHitBounds;

        var overlappingObjects = Physics.OverlapBox(hitBounds.center, hitBounds.size);
        List<GameObject> targetsInRange = new List<GameObject>();
        var length = overlappingObjects.Length;
        for (var index = 0; index < length; index++)
        {
            var collider = overlappingObjects[index];
            if (!IsTargetValid(collider)) continue;
            targetsInRange.Add(collider.gameObject);
        }

        return targetsInRange;
    }

    private bool IsTargetValid(Collider collider)
    {
        GameObject objectInRange = collider.gameObject;

        return objectInRange != null && IsHittable(objectInRange) && !objectInRange.Equals(gameObject) &&
               objectInRange.HasTag("Player");
    }

    private bool IsHittable(GameObject targetObject)
    {
        return targetObject.GetComponent<IDamagable>() != null;
    }

    private void AddHit(GameObject target)
    {
        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable == null) return;
        OnHitTarget(damagable, target);
        _hitSystem.targetCollisions.Add(target);
    }

    private void OnHitTarget(IDamagable damagable, GameObject target)
    {
        var forceIntensity = (_entityStats.Strength * EnemyAI.forceBody.Mass);
        var direction = Vector3.one;
        var attackVelocity = EnemyAI.forceBody.Velocity.normalized;
        direction.x = Mathf.Max(Mathf.Sign(attackVelocity.x));
        
        KnockbackSystem.Instance.AddKnockback(target, true, KnockbackIds.DroneCharge, forceIntensity,direction, gameObject.transform);
        damagable.TakeDamage();
        target.TakeDamage(damageAmount);
    }
}
