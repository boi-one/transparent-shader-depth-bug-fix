using System;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    private Vector3 _startPosition;
    private float _maxDistance;
    private Vector3 _direction;
    private float _speed;
    private bool _hasHitObject;
    
    public UnityEvent<string, Quaternion> onHitEnter = new UnityEvent<string, Quaternion>();
    public UnityEvent onHitEnterLeft = new UnityEvent();
    public UnityEvent onHitEnterRight = new UnityEvent();
    
    [SerializeField] private string impactForceName;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float hitOffset = 1f;
    [SerializeField] private string impactParticleName = "go_impact_bullet";

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (_hasHitObject) return;

        if (Physics.SphereCast(transform.position, radius, Velocity, out var hit, Velocity.magnitude))
        {
            var lastStep = (hit.point - transform.position).magnitude;
            transform.position += Velocity.normalized * (hitOffset+lastStep);
            CheckHit(hit.collider.gameObject);
        }
        else
        {
            transform.position += Velocity;
        }
        
        if (Vector3.Distance(_startPosition, transform.position) >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetMaxDistance(float maxDistance)
    {
        _maxDistance = maxDistance;
    }
    
    public Vector3 Velocity { get; set; }

    public void OnHit(GameObject hitObject)
    {
        if(hitObject.HasComponent<HealthData>())
        {
            hitObject.GetComponent<HealthData>().TakeDamage();
            //KnockbackSystem.Instance.AddKnockback(hitObject, _direction.x, KnockbackIds.ProjectileHit);
        }
        
        var impactRotation = Quaternion.FromToRotation(Vector3.right, -Velocity);
        onHitEnter.Invoke(impactParticleName, impactRotation);

        _hasHitObject = false;
        
        if (this._direction.x >= 0) onHitEnterRight?.Invoke();
        if (this._direction.x < 0) onHitEnterLeft?.Invoke();
        
        Destroy(gameObject);
    }

    public void CheckHit(GameObject other)
    {
        if (_hasHitObject) return;
        _hasHitObject = true;
        OnHit(other.gameObject);
        other.TakeDamage(damage);
    }
}