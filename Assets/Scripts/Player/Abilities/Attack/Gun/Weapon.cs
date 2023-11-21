using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Weapon : Ability
{
    [SerializeField] protected float fireRate = 0.33f;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Vector3 shootDir = new Vector3(1, 0, 0);
    [SerializeField] protected float speed = 80f;
    [SerializeField] protected float maxShootDistance = 64f;
    [SerializeField] private Vector3 scaleVector;
    private TargetSelector _targetSelector;
    private ParticleManager particle;

    protected bool _isOnCoolDown;
    protected Rigidbody _bulletRb;

    private void Start()
    {
        particle = new ParticleManager();
        _targetSelector = GetComponent<TargetSelector>();

        _targetSelector.Origin = firePoint;
        _targetSelector.Setup(firePoint);
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (_isOnCoolDown || !CanUseAbility) return;

        shootDir = _targetSelector.CurrentDirection;

        onUseAbility?.Invoke();
        StartCoroutine(ShootBullet());
    }

    protected IEnumerator ShootBullet()
    {
        _isOnCoolDown = true;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        bulletComponent.Velocity = shootDir.normalized * speed;

        bulletComponent.SetMaxDistance(maxShootDistance);

        yield return new WaitForSeconds(fireRate);
        _isOnCoolDown = false;
    }
    
}
