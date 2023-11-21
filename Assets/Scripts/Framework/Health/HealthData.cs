using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthData : MonoBehaviour, IDamagable
{
    [SerializeField] private bool isInvulnerable;
    [SerializeField] private float health = 100f;
    private float _maxHealth;
    
    [Header("Unity Events")]
    public UnityEvent<HealthEvent> onHealthChanged = new UnityEvent<HealthEvent>();
    [SerializeField] private UnityEvent onHealthAdded = new UnityEvent();
    [SerializeField] private UnityEvent onDamageTaken = new UnityEvent();
    [SerializeField] private UnityEvent onDie = new UnityEvent();
    [SerializeField] private UnityEvent onResurrected = new UnityEvent();
    [SerializeField] private UnityEvent onSelfDestruct = new UnityEvent();

    public float Health => health;
    public float MaxHealth => _maxHealth;
    public bool HasMaxHealth => health >= _maxHealth;
    
    private bool _isHit;
    private bool _isDead;

    public bool IsInvulnerable
    {
        get => isInvulnerable;
        set => isInvulnerable = value;
    }

    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }

    private void Start()
    {
        InitHealth();
    }

    private void InitHealth()
    {
        _maxHealth = health;
    }

    public void AddHealth(float healthAmount)
    {
        if (_isDead || HasMaxHealth) return;

        health += healthAmount;

        onHealthAdded?.Invoke();
        TriggerChangedEvent(HealthEventTypes.AddHealth, healthAmount);
    }

    private void TriggerChangedEvent(HealthEventTypes type, float healthDelta = 0)
    {
        var healthEvent = new HealthEvent()
        {
            type = type,
            target = gameObject,
            currenthealth = Health,
            healthDelta = healthDelta,
            maxHealth = MaxHealth
        };
        onHealthChanged?.Invoke(healthEvent);
    }

    public void Resurrect(float newHealth)
    {
        _isDead = false;
        AddHealth(newHealth);
        onResurrected?.Invoke();
        
        TriggerChangedEvent(HealthEventTypes.Resurrect, newHealth);
    }

    public void TakeDamage(float damage = 0)
    {
        if (_isDead || _isHit || isInvulnerable) return;

        _isHit = true;
        health -= damage;
        onDamageTaken?.Invoke();
        TriggerChangedEvent(HealthEventTypes.TakeDamage, damage);
        _isHit = false;
        
        if (health <= 0) Die();
    }

    private void Die()
    {
        health = 0;
        _isDead = true;
        onDie?.Invoke();
        TriggerChangedEvent(HealthEventTypes.Die);
    }

    public void Kill()
    {
        Die();
    }

    public void SelfDestruct()
    {
        onSelfDestruct?.Invoke();
        Destroy(gameObject);
    }
}