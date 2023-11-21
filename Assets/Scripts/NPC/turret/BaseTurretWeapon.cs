using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseTurretWeapon : MonoBehaviour
{
    
    public UnityEvent onShoot = new UnityEvent();
    public UnityEvent onStopShooting = new UnityEvent();
    public UnityEvent onStartWeaponFire = new UnityEvent();
    public UnityEvent<GameObject> onDamage = new UnityEvent<GameObject>();
    public UnityEvent onAvailable = new UnityEvent();
    public virtual void Shoot()
    {
        onShoot?.Invoke();
    }

    public virtual void StopShoot()
    {
        onStopShooting?.Invoke();
    }
}
