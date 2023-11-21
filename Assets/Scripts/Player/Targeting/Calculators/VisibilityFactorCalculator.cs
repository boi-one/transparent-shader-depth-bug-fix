using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityFactorCalculator : PriorityFactorCalculator
{
    [SerializeField] private AutomaticTargetingSystem automaticTargetingSystem;
    
    public override float CalculateWeight(Selectable selectable, float factorWeight)
    {
        var weight = Mathf.InverseLerp(0, 1, Vector2.Dot((selectable.GetPosition() - transform.position).normalized, automaticTargetingSystem.GetDirection()));
        
        return weight * factorWeight;
    }
}