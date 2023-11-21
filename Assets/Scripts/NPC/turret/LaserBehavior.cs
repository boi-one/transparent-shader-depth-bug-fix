using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LaserBehavior : BaseTurretWeapon
{
    [SerializeField] private LayerMask blockingLayer;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private float shootDuration;
    [SerializeField] private int radius;
    [SerializeField] private float chargeTime;
    [SerializeField] private float cooldownTime;
    [SerializeField] private int weaponDamage;
    
    public RaycastHit hit;
    public TurretStates CurrentState { get; set; } = TurretStates.Idle;
    public UnityEvent onStartCharging = new UnityEvent();
    public UnityEvent<Vector3, Vector3, RaycastHit> onLaserUpdate = new UnityEvent<Vector3, Vector3, RaycastHit>();
    
    private Transform _currentTransform;
    
    private void Start()
    {
        _currentTransform = transform;
    }
    public override void Shoot()
    {
        if (CurrentState != TurretStates.Idle) return;
        
        base.Shoot();
        Charge();
    }

    private void Charge()
    {
        CurrentState = TurretStates.Charging;
        onStartCharging?.Invoke();
        Invoke("StartLaser", chargeTime);
    }

    private void StartLaser()
    {
        CurrentState = TurretStates.Shooting;
        onStartWeaponFire?.Invoke();
        Invoke("StopShoot", shootDuration);
    }

    public override void StopShoot()
    {
        base.StopShoot();
        StartCooldown();
        onStopShooting?.Invoke();
    }

    private void StartCooldown()
    {
        CurrentState = TurretStates.Cooldown;
        Invoke("Reset", cooldownTime);
    }

    private void Reset()
    {
        CurrentState = TurretStates.Idle;
        onAvailable?.Invoke();
    }

    private void Update()
    {
        if (CurrentState != TurretStates.Shooting) return;
        UpdateLaser();
    }
    
    private void UpdateLaser()
    {
        var startPoint = transform.position;
        var direction = _currentTransform.right;
        if (!Physics.SphereCast(startPoint, radius, direction, out hit, Mathf.Infinity, blockingLayer)) return;
        onLaserUpdate?.Invoke(transform.position, hit.point, hit);
        DealDamage();
    }
    private void DealDamage()
    {
        var startPoint = transform.position;
        var direction = _currentTransform.right;

        var damageableColliders = Physics.SphereCastAll(startPoint, radius, direction, hit.distance, damageLayer);
        for (int i = 0; i < damageableColliders.Length; i++)
        {
            var targetGameObject = damageableColliders[i].collider.gameObject;
            targetGameObject.TakeDamage(weaponDamage);
            onDamage?.Invoke(targetGameObject);
        }
    }
}
