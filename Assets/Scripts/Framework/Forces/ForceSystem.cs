using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;


public class ForceSystem : GenericSingleton<ForceSystem>
{
    private ForceBody _currentForceBody;

    [Header("Settings")] 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 gravity = new Vector3(0, -2.5f, 0);
    [SerializeField] private float minResidualForce = 0.01f;

    [Header("Active Forcebodies")]
    [SerializeField] private List<ForceBody> forceBodies = new List<ForceBody>();

    private Vector3 _allowedAxisModifier = new Vector3(1, 1, 0); // todo: in de editor 3 booleans mogelijk maken
    private List<Collider> _disabledColliders = new List<Collider>();

    [Header("OneWay Collision")]
    private Dictionary<Collider, float> _oneWayAccesAngles = new Dictionary<Collider, float>();
    private HashSet<Collider> _oneWayCollidersCache  = new HashSet<Collider>();

    [Header("CollisionEvent")] 
    private Dictionary<Transform, CollisionEventDispatcher> _collisionEventCache = new Dictionary<Transform, CollisionEventDispatcher>();
    private HashSet<Transform> _hitCache  = new HashSet<Transform>();
    private HashSet<Transform> _lastFrameHitCache  = new HashSet<Transform>();
    
    [Header("Events")]
    public UnityEvent<ForceBody> onForceBodyAdd = new UnityEvent<ForceBody>();
    public UnityEvent<ForceBody> onForceBodyRemove = new UnityEvent<ForceBody>();
    public UnityEvent<List<ForceBody>> onForceBodiesChange = new UnityEvent<List<ForceBody>>();

    public LayerMask GroundLayer => groundLayer;

    private void Start()
    {
        // emit start forces
        onForceBodiesChange?.Invoke(forceBodies);
    }

    public void AddBody(ForceBody forceBody)
    {
        forceBodies.Add(forceBody);
        ForceBodiesChanged(forceBody);
    }

    public void RemoveBody(ForceBody forceBody)
    {
        forceBodies.Remove(forceBody);
        ForceBodiesChanged(forceBody);
    }

    private void ForceBodiesChanged(ForceBody delta)
    {
        onForceBodiesChange?.Invoke(forceBodies);
        if (forceBodies.Contains(delta))
        {
            onForceBodyAdd?.Invoke(delta);
        }
        else
        {
            onForceBodyRemove?.Invoke(delta);
        }
    }

    private void FixedUpdate()
    {
        if (!IsEnabled) return;

        var l = forceBodies.Count;
        for (int i = 0; i < l; i++)
        {
            if (forceBodies[i].IsEnabled) UpdateBody(forceBodies[i]);
        }
        
        ResolveLeaveEvents();
    }

    private void UpdateBody(ForceBody forceBody)
    {
        _currentForceBody = forceBody;

        // todo: zorgen dat we dit niet elke fixed update hoeven te doen maar alleen bij mutatie van _activeForceables en _gravityForceables
        _currentForceBody.ConcatCurrentForceables();
        var l = _currentForceBody.CurrentForceables.Count;
        if (l == 0) return;

        _currentForceBody.ResetVelocity();

        if (_currentForceBody.hasGravity) ApplyGravity();

        for (int i = l - 1; i >= 0; i--)
        {
            
            var currentForceable = _currentForceBody.CurrentForceables[i];
            UpdateForce(currentForceable);
            ApplyForce(currentForceable);
            if (IsCompleted(currentForceable))
            {
                _currentForceBody.EditCallback(_currentForceBody.callbacks, currentForceable, config => config.OnCancel = null);
                _currentForceBody.Remove(currentForceable);
                _currentForceBody.Callback(_currentForceBody.callbacks, currentForceable, config => config.OnFinish?.Invoke(), true);
                if (currentForceable.Type != ForceTypes.SingleAddition) AddResidualForce(currentForceable);
            }
        }

        ResolveCollisions();
        ResolveAllowedAxis();

        Move();
    }

    private void ResolveAllowedAxis()
    {
        var velocity = _currentForceBody.Velocity;
        velocity.x *= _allowedAxisModifier.x;
        velocity.y *= _allowedAxisModifier.y;
        velocity.z *= _allowedAxisModifier.z;
        _currentForceBody.Velocity = velocity;
    }

    private void Move()
    {
        _currentForceBody.transform.position += _currentForceBody.Velocity;
    }

    private void ResolveCollisions()
    {
        var center = _currentForceBody.transform.position;
        center.y += _currentForceBody.radius;

        var velocity = _currentForceBody.Velocity;
        _currentForceBody.DesiredVelocity = velocity;

        var currentPos = center;

        RaycastHit hit = default;
        RaycastHit firstHit = default;

        var remainingLength = velocity.magnitude;
        var maxLoopCount = 40;
        var i = 0;
        var margin = 0.01f;
        var fullAbsorptionRange = 0.1f;
        var dampingRange = 1f - fullAbsorptionRange;
        var isColliding = false;
        var disableGravity = false;
        _disabledColliders.Clear();

        while (i < maxLoopCount && remainingLength > 0)
        {
            i++;
            if (Physics.SphereCast(center, _currentForceBody.radius, velocity, out hit, velocity.magnitude, _currentForceBody.CollisionLayer))
            {
                if (CheckOneWayCollision(hit))
                {
                    DisableCollider(hit.collider);
                    continue;
                }

                if (firstHit.collider == null) firstHit = hit;

                HandleCollisionEvents(hit, _currentForceBody.gameObject);

                var step = (hit.point + _currentForceBody.radius * hit.normal) - center;
                step = step.normalized * (step.magnitude - margin);
                center = center + step;
                remainingLength -= step.magnitude;

                var frictionDirection = Vector2.Perpendicular(hit.normal);

                if (Vector2.Dot(frictionDirection.normalized, velocity.normalized) < 0)
                    frictionDirection *= -1;

                var dampingPower = Math.Abs(Vector2.Dot(velocity.normalized, hit.normal)) / dampingRange;
                dampingPower = Mathf.Min(dampingPower, 1);

                if (hit.normal.y > 0.85)
                {
                    disableGravity = true;
                }

                remainingLength *= 1 - dampingPower;
                isColliding = true;
                velocity = frictionDirection.normalized * remainingLength;
            }
            else
            {
                center += velocity;
                break;
            }
        }
        EnableColliders();

        var colliders = Physics.OverlapSphere(center, _currentForceBody.radius, groundLayer);
        if (colliders.Length > 0 && colliders[0].name != _currentForceBody.name && !colliders[0].CompareTag("OneWay"))
        {
            center = currentPos;
        }

        _currentForceBody.Velocity = center - currentPos;

        if (isColliding) OnCollide(disableGravity, firstHit);
    }

    private void UpdateForce(IForceable targetForceable)
    {
        targetForceable.Update(Time.fixedDeltaTime);
        if (IsCompleted(targetForceable))
        {
            targetForceable.Finish();
        }
    }

    private void AddResidualForce(IForceable targetForceable)
    {
        if (targetForceable.IgnoresResidualForce || targetForceable.ResidualForce.magnitude < minResidualForce) return;

        _currentForceBody.Add(new Force()
        {
            Direction = targetForceable.ResidualForce,
            Type = ForceTypes.Collision
        });
    }

    private bool IsCompleted(IForceable targetTween)
    {
        return targetTween.Type == ForceTypes.SingleAddition || Math.Abs(targetTween.Interpolation - 1f) < 0.00000000001f;
    }

    private void ApplyForce(IForceable targetTween)
    {
        _currentForceBody.Velocity += targetTween.CurrentForce;
    }

    private void ApplyGravity()
    {
        var l = _currentForceBody.GravityForceables.Count;
        for (int i = 0; i < l; i++)
        {
            _currentForceBody.GravityForceables[i].Direction += gravity * (_currentForceBody.GravityScale * Time.fixedDeltaTime);
        }
    }

    private void ResolveLeaveEvents()
    {
        CollisionEventDispatcher collisionEventDispatcher;
        foreach (Transform currentTransform in _lastFrameHitCache)
        {
            if (!(!_hitCache.Contains(currentTransform) && _collisionEventCache.TryGetValue(currentTransform, out collisionEventDispatcher)))
                continue;
            
            collisionEventDispatcher.OnLeave();
            _collisionEventCache.Remove(currentTransform);
        }
        
        _lastFrameHitCache = _hitCache;
        _hitCache = new HashSet<Transform>();
    }

    private void HandleCollisionEvents(RaycastHit hit, GameObject collider)
    {
        _hitCache.Add(hit.transform);

        CollisionEventDispatcher collisionEventDispatcher;
        _collisionEventCache.TryGetValue(hit.transform, out collisionEventDispatcher);
        
        if (collisionEventDispatcher != null)
        {
            collisionEventDispatcher.OnStay();
        }
        else 
        {
            collisionEventDispatcher = hit.transform.GetComponent<CollisionEventDispatcher>();
            if (collisionEventDispatcher == null)
                return;

            if (collisionEventDispatcher.AllowedLayers != (collisionEventDispatcher.AllowedLayers | (1 << collider.layer)))
                return;
            
            collisionEventDispatcher.OnEnter();
            _collisionEventCache.Add(hit.transform, collisionEventDispatcher);
        }
    }

    private bool CheckOneWayCollision(RaycastHit hit)
    {
        var currentCollider = hit.collider;
        var currentBodyDirection = _currentForceBody.DesiredVelocity.normalized;

        if (!_oneWayCollidersCache.Contains(currentCollider))
        {
            _oneWayCollidersCache.Add(currentCollider);

            if (currentCollider.TryGetComponent(out OneWayPlatform oneWayPlatform))
                _oneWayAccesAngles.Add(currentCollider, oneWayPlatform.accessAngle);
        }

        if (_oneWayAccesAngles.ContainsKey(currentCollider))
        {
            var accessThreshold = 1 - _oneWayAccesAngles[currentCollider] / 180;
            var comparedAngle = Vector3.Dot(hit.transform.up, currentBodyDirection);
            if (comparedAngle >= accessThreshold) return true;
        }

        return false;
    }
    
    private void DisableCollider(Collider targetCollider)
    {
        targetCollider.enabled = false;
        _disabledColliders.Add(targetCollider);
    }

    private void EnableColliders()
    {
        var l = _disabledColliders.Count;
        for (int j = 0; j < l; j++)
        {
            _disabledColliders[j].enabled = true;
        }
    }

    void OnCollide(bool disableGravity, RaycastHit hit)
    {
        _currentForceBody.Remove(ForceTypes.Collision);
        _currentForceBody.OnCollision(hit);

        var collsionEvent = new CollisionForceEvent()
        {
            type = CollisionEventTypes.Grab,
            resetsGravity = disableGravity
        };

        _currentForceBody.onCollisionEvent.Invoke(collsionEvent);

        if (disableGravity)
            _currentForceBody.ResetGravity();
    }

    public bool CheckCollision(ForceBody forceBody, Vector3 direction, float distance, LayerMask layer)
    {
        var center = forceBody.transform.position;
        center.y += forceBody.radius;

        return Physics.SphereCast(center, forceBody.radius, direction, out var hit, distance, layer);
    }

    public bool CheckFrontalCollision(ForceBody forceBody, Vector3 direction, float distance, LayerMask layer, float threshold)
    {
        var center = forceBody.transform.position;
        center.y += forceBody.radius;

        var hits = Physics.SphereCastAll(center, forceBody.radius, direction, distance, layer);

        foreach (var hit in hits)
        {
            if (hit.normal.IsOppositeDirectionAs(direction, threshold)) return true;
        }

        return false;
    }

    public bool IsEnabled { get; set; } = true;
}