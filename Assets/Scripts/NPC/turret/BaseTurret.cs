using UnityEngine;

public class BaseTurret : MonoBehaviour
{
    
    [SerializeField] protected BaseTurretWeapon turretWeapon;
    [SerializeField] private float shootInterval;
    [SerializeField] private float shootIntervalDelay;

    [SerializeField] private bool isAutoShooting;

    public bool IsAutoShooting
    {
        get => isAutoShooting;
        set
        {
            isAutoShooting = value;
            if (!isAutoShooting) StopShooting();
            else StartShootInterval();
        }
    }

    private void StopShooting()
    {
        CancelInvoke();
    }

    protected void Awake()
    {
        if (!isAutoShooting) return;

        StartShootInterval();
    }

    private void StartShootInterval()
    {
        StopShooting();
        InvokeRepeating("Shoot", shootIntervalDelay, shootInterval);
    }

    public void Shoot()
    {
        turretWeapon.Shoot();
    }

}
