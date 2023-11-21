using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistanceFactorCalculator : PriorityFactorCalculator
{
    [SerializeField] private AutomaticTargetingSystem automaticTargetingSystem;
    
    public override float CalculateWeight(Selectable selectable, float factorWeight)
    {
        var weight = 1 - Mathf.InverseLerp(0, automaticTargetingSystem.Selectables.OrderBy(s => s.GetDistance(transform.position)).LastOrDefault().GetDistance(transform.position),
            Vector2.Distance(transform.position, selectable.GetPosition()));
        
        return weight * factorWeight;
    }
}