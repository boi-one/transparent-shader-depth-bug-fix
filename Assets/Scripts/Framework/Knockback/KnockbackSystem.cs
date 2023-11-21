using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Pathfinding;
using UnityEngine;

/// <summary>
/// First check to see if obj has custom knockback settings -----> Use it
///     |
///     | No?
///     |
///     ----> Try to use a knockback preset with the same type of knockback (enum)
///                |
///                | No?
///                |
///                -----> Then add default knockback if obj is knockable
/// </summary>

public class KnockbackSystem : GenericSingleton<KnockbackSystem>
{
    private Dictionary<KnockbackIds, KnockbackSetting> _knockbackCollection = new Dictionary<KnockbackIds, KnockbackSetting>();
    [SerializeField] private List<KnockbackSetting> knockbackList = new List<KnockbackSetting>();
    [SerializeField] private Force defaultKnockbackForce;
    [SerializeField] private Force bounceKnockbackForce;
    [SerializeField] private float groundCheckMargin = 0.1f;
    [SerializeField] private float bounceThreshold = 0.01f;

    private Dictionary<ForceBody, int> _bounceCount = new Dictionary<ForceBody, int>();
    private Dictionary<ForceBody, int> _groundBounceCount = new Dictionary<ForceBody, int>();

    private float _groundCheckMargin;
    private int _groundLayer = 6;

    private void Awake()
    {
        _groundCheckMargin = 1 - groundCheckMargin;

        //copy knockback list to dictionary 
        foreach (KnockbackSetting knockbackSetting in knockbackList)
        {
            _knockbackCollection.Add(knockbackSetting.knockbackId, knockbackSetting);
        }
    }
    public void AddKnockback(GameObject target, bool flipDirection, KnockbackIds type, float forceIntensity = 1f, Vector2 forceDirection = default, Transform knockbackOrigin = null)
    {
        AddKnockback( target, flipDirection ? 1f : -1f,  type,  forceIntensity,  forceDirection,  knockbackOrigin);
    }

    public void AddKnockback(GameObject target, float xDirection, KnockbackIds type, Transform knockbackOrigin = null)
    {
        AddKnockback(target, xDirection, type, 1f, Vector2.one, knockbackOrigin);
    }

    //call this function to add knockback to something
    public void AddKnockback(GameObject target, float xDirection, KnockbackIds type, float forceIntensity = 1f, Vector2 forceDirection = default, Transform knockbackOrigin = null)
    {
        if (forceDirection == default) forceDirection = Vector2.one;
        bool isKnockable = true;
        bool leftRight;


        if (Mathf.RoundToInt(xDirection) == 1) leftRight = true;
        else leftRight = false;

        var currentKnockBackForce = defaultKnockbackForce;

        if (HasCustomSettings(target))
        {
            isKnockable = target.GetComponent<CustomKnockback>().isKnockable;
            currentKnockBackForce = GetCustomForceSettings(target);
        }
        else if (_knockbackCollection.ContainsKey(type))
        {
            currentKnockBackForce = _knockbackCollection[type].knockbackForce;
        }

        ApplyKnockback(isKnockable, leftRight, target, currentKnockBackForce, forceIntensity, forceDirection, knockbackOrigin);
    }

    //apply the correct force
    private void ApplyKnockback(bool isKnockable, bool leftRight, GameObject target, Force force, Transform knockbackOrigin = null)
    {
        ApplyKnockback(isKnockable, leftRight, target, force, 1f, Vector2.one, knockbackOrigin);
    }

    private void ApplyKnockback(bool isKnockable, bool leftRight, GameObject target, Force force, float forceIntensity = 1f, Vector2 forceDirection = default, Transform knockbackOrigin = null)
    {
        if (!isKnockable) return;

        var newForce = force.Clone();

        var knockbackDirection = FixDirection(leftRight, newForce);

        var forceBody = target.GetComponent<ForceBody>();
        if (forceBody == null) return;

        newForce.Id = ForceIds.Knockback;
        newForce.Direction = knockbackDirection * forceDirection * forceIntensity / forceBody.Mass;


        forceBody.Add(newForce);
        _bounceCount[forceBody] = 0;
        _groundBounceCount[forceBody] = 0;

        TriggerCollisionEvent(forceBody, CollisionEventTypes.Knockback, knockbackDirection, new RaycastHit(), false, knockbackOrigin);
    }

    public void BounceKnockback(ForceBody forceBody, RaycastHit hit)
    {
        if (GetBounceSetting(forceBody.gameObject, out var bounceSetting))
        {
            if (bounceSetting.Bounciness <= 0) return;
        }
        else return;

        if (!bounceSetting.CanBounce) return;

        if (GetBounceSetting(hit.transform.gameObject, out var hitBounceSetting))
        {
            if (hitBounceSetting.OnBounceBounciness <= 0) return;

            var hitForceBody = hit.transform.gameObject.GetComponent<ForceBody>();

            ApplyBounce(hitForceBody, forceBody.DesiredVelocity, hitBounceSetting.OnBounceBounciness, hitBounceSetting.BounceDecreaseMultiplier, hitBounceSetting.MinBouncesUntilDecrease, hit, false);
        }
        var bounceDirection = Vector3.Reflect(forceBody.DesiredVelocity, hit.normal);
        ApplyBounce(forceBody, bounceDirection, bounceSetting.Bounciness, bounceSetting.BounceDecreaseMultiplier, bounceSetting.MinBouncesUntilDecrease, hit, true);
    }

    private void ApplyBounce(ForceBody forceBody, Vector3 direction, float bouncinessMultiplier, float bounceDecreaseMultiplier, float minNormalBounces, RaycastHit hit, bool doCheckVelocity)
    {
        if (doCheckVelocity && forceBody.DesiredVelocity.sqrMagnitude < bounceThreshold) return;

        // todo: cleanup en verder uitdenken
        var desiredVelocity = forceBody.DesiredVelocity;
        desiredVelocity = desiredVelocity.sqrMagnitude > 0 ? desiredVelocity.normalized * direction.magnitude : direction;
        forceBody.DesiredVelocity = desiredVelocity;

        if (!_bounceCount.ContainsKey(forceBody)) _bounceCount[forceBody] = 0;

        var bounceDirection = direction.normalized * (_bounceCount[forceBody] > minNormalBounces - 1 ? bounceDecreaseMultiplier : bouncinessMultiplier);

        bool isGroundHit = IsBounceGrounded(hit);
        if (isGroundHit)
        {
            if (!_groundBounceCount.ContainsKey(forceBody)) _groundBounceCount[forceBody] = 0;

            _groundBounceCount[forceBody]++;
        }

        TriggerCollisionEvent(forceBody, CollisionEventTypes.Bounce, bounceDirection, hit, isGroundHit);

        // todo: ombouwen zodat je een boolean kunt aangeven bij de force dat hij de forces moet clearen
        forceBody.ClearForces();

        var newHitForce = bounceKnockbackForce.Clone();
        newHitForce.Direction = bounceDirection;

        var magnitude = newHitForce.Direction.sqrMagnitude;

        _bounceCount[forceBody] += 1;
        forceBody.Add(newHitForce);
    }

    private void TriggerCollisionEvent(ForceBody forceBody, CollisionEventTypes eventType, Vector3 newDirection, RaycastHit hit = new RaycastHit(), bool isGroundHit = false, Transform collisionOrigin = null)
    {
        var collisionEvent = new CollisionForceEvent()
        {
            type = eventType,
            velocity = forceBody.Velocity,
            newDirection = newDirection,
            hit = hit,
            targetForcebody = forceBody,
            isGroundHit = isGroundHit,
            collisionOrigin = collisionOrigin
        };

        forceBody.OnCollisionEvent(collisionEvent);
    }

    private bool IsBounceGrounded(RaycastHit hit)
    {
        if (hit.collider.gameObject.layer != _groundLayer) return false;

        float upDot = Vector3.Dot(Vector3.up, hit.normal);
        return upDot >= _groundCheckMargin;
    }

    public void OnKnockbackBounceCompleted(ForceBody forceBody)
    {
        _bounceCount.Remove(forceBody);
        _groundBounceCount.Remove(forceBody);
    }

    private Vector3 FixDirection(bool leftRight, Force force)
    {
        var knockbackDir = force.Direction;
        if (!leftRight) knockbackDir.x = -knockbackDir.x;
        return knockbackDir;
    }

    private Vector2 FixDirection(bool leftRight, Vector2 forceDirection)
    {
        var knockbackDir = forceDirection;
        if (!leftRight) knockbackDir.x = -knockbackDir.x;
        return knockbackDir;
    }

    private bool HasCustomSettings(GameObject target)
    {
        return target.HasComponent<CustomKnockback>();
    }

    private Force GetCustomForceSettings(GameObject target)
    {
        return target.GetComponent<CustomKnockback>().customForce;
    }

    private bool GetBounceSetting(GameObject target, out BounceSetting bounceSetting)
    {
        bounceSetting = target.GetComponent<BounceSetting>();
        return bounceSetting != null;
    }

}
