using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealthExtentions
{
    private static bool GetHealthComponent(this GameObject gameObject, out HealthData healthData)
    {
        healthData = gameObject.GetComponent<HealthData>();
        return healthData != null;
    }

    public static bool AddHealth(this GameObject gameObject, float health)
    {
        if (!GetHealthComponent(gameObject, out var healthComponent)) return false;
        healthComponent.AddHealth(health);
        return true;
    }

    public static bool TakeDamage(this GameObject gameObject, float health)
    {
        if (!GetHealthComponent(gameObject, out var healthComponent)) return false;
        healthComponent.TakeDamage(health);
        return true;
    }

    public static bool Resurrect(this GameObject gameObject, float newHealth)
    {
        if (!GetHealthComponent(gameObject, out var healthComponent)) return false;
        healthComponent.Resurrect(newHealth);
        return true;
    }

    public static bool IsDead(this GameObject gameObject)
    {
        if (!GetHealthComponent(gameObject, out var healthComponent)) return false;

        var isDead = healthComponent.Health <= 0;
        return isDead;
    }
    
    public static bool Kill(this GameObject gameObject)
    {
        if (!GetHealthComponent(gameObject, out var healthComponent)) return false;

        healthComponent.Kill();
        return true;
    }
}