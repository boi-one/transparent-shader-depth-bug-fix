using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFactorCalculator : PriorityFactorCalculator
{
    [SerializeField] private AutomaticTargetingSystem automaticTargetingSystem;
        
    public override float CalculateWeight(Selectable selectable, float factorWeight)
    {
        var weight = selectable.transform.parent.TryGetComponent<HealthData>(out var healthData) ? 1 - Mathf.InverseLerp(0, GetComponent<HealthData>().Health, healthData.Health) : 0;
        
        return weight * factorWeight;
    }
}