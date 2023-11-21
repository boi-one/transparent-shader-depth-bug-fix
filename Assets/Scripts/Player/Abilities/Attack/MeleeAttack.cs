using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;
using StateMachine.PlayerInputHandlers;
using UnityEngine.Serialization;

public class MeleeAttack : Ability
{
    [SerializeField] protected UnityEvent onHitATarget = new UnityEvent();
    [SerializeField] protected UnityEvent<GameObject> onHitTargets = new UnityEvent<GameObject>();
    [SerializeField] private float damage = 10f;
    [SerializeField] private Vector3 hitArea = new Vector3(1, 1, 0);
    [SerializeField] private Vector3 hitAreaOffset = new Vector3(1,0,0);
    [SerializeField] private int direction = 1;
    [SerializeField] private float fireRate = 0.75f;

    [SerializeField] private ComboSystem comboSystem;
    [SerializeField] private EntityStats entityStats;
    [SerializeField] private ForceBody forceBody;

    private bool _isOnCoolDown;

    [SerializeField] private string impactForceName = "Hit";

    private HitSystem _hitSystem;

    public void Start()
    {
        _hitSystem = GetComponent<HitSystem>();

    }
    public override bool Use(InputAction.CallbackContext context)
    {
        if (CanUseAbility == false) return false;
        if (_isOnCoolDown) return false;
        _isOnCoolDown = true;
        StartCoroutine(MeleeHit());
        
        EmitUse();
        Direction = GetLookDirection();

        return true;
    }

    private void FixedUpdate()
    {
        if (!_hitSystem.IsPerformingAttack) return;
        
        foreach (GameObject target in GetTargetsInRange())
        {
            if(_hitSystem.CheckHasHit(target)) continue;
            
            AddHit(target);
        }
    }

    public int GetLookDirection()
    {
        return transform.localScale.x == 1 ? 1 : -1;
    }
    
    private List<GameObject> GetTargetsInRange()
    {
        
        var offset = hitAreaOffset;
        offset.x *= direction;
        
        var hitBounds = _hitSystem.CurrentHitBounds;

        var overlappingObjects = Physics.OverlapBox( hitBounds.center, hitBounds.size);
        List<GameObject> targetsInRange = new List<GameObject>();
        foreach (Collider collider in overlappingObjects)
        {
            if (!IsTargetValid(collider)) continue;
            targetsInRange.Add(collider.gameObject);
            
        }
        return targetsInRange;
    }

    private void AddHit(GameObject target)
    {
        IDamagable damagable = target.GetComponent<IDamagable>();
        if(damagable == null) return;
        OnHitTarget(damagable, target);
        _hitSystem.targetCollisions.Add(target);
    }

    private bool IsTargetValid(Collider collider)
    {
        GameObject objectInRange = collider.gameObject;

        return objectInRange != null && IsHittable(objectInRange) && !objectInRange.Equals(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        if(_hitSystem == null) return;
        
        Gizmos.color = _hitSystem.IsPerformingAttack ? Color.red : Color.green;
        var offset = hitAreaOffset;
        offset.x *= direction;

        Physics.SyncTransforms();
        
        var bounds = _hitSystem.CurrentHitBounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
    
    public bool IsHittable(GameObject targetObject)
    {
        return targetObject.GetComponent<IDamagable>() != null;
    }

    private void OnHitTarget(IDamagable damagable, GameObject target)
    {
        if (!_hitSystem.HasHitATarget) onHitATarget?.Invoke();
        onHitTargets?.Invoke(target);
        
        // probably not needed anymore, due to combosnap
        //var currentComboStep = comboSystem.currentComboAttack;
        //var forceIntensity = (entityStats.Strength * forceBody.Mass) * currentComboStep.forceIntensity;
        //KnockbackSystem.Instance.AddKnockback(target,  GetLookDirection(), KnockbackIds.MeleeAttack, forceIntensity, currentComboStep.forceDirection, gameObject.transform);
        damagable.TakeDamage(damage);
        target.TakeDamage(damage);
    }

    public int Direction
    {
        get => direction;
        set => direction = value;
    }

    public override void OnInput(InputAction.CallbackContext aContext)
    {
        Use(aContext);
        GetComponentInChildren<StateMachine.StateMachine>().SetBool("Attacking", true);
    }

    IEnumerator MeleeHit()
    {
        yield return new WaitForSeconds(fireRate);
        _isOnCoolDown = false;
    }
    
}