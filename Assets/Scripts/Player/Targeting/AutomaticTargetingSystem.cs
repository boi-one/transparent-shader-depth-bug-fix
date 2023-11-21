using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutomaticTargetingSystem : CameraTargetSelector, IModifiable
{
    [Serializable]
    public struct PriorityFactor
    {
        public string name;
        public PriorityFactorCalculator factorCalculator;
        public float factorWeight;
    }
    
    [Serializable]
    public struct PriorityGroup
    {
        public string name;
        public List<PriorityFactor> priorityFactors;
    }

    [SerializeField] private List<PriorityGroup> priorityGroups = new List<PriorityGroup>();

    private int _currentPriorityGroup;

    private Dictionary<Selectable, float> weights = new Dictionary<Selectable, float>();

    public void Modify(int index)
    {
        _currentPriorityGroup = index;
    }

    protected override void DetermineTarget()
    {
        weights.Clear();
        
        foreach (var selectable in GetValidSelectables(direction))
        {
            float weight = 0;
        
            priorityGroups[_currentPriorityGroup].priorityFactors.ForEach(factor => weight += factor.factorCalculator.CalculateWeight(selectable, factor.factorWeight));
            
            weights.Add(selectable, weight);
        }
        
        var target = weights.OrderBy(selectable => selectable.Value).LastOrDefault().Key;

        if (target != null) Select(target);
        else Deselect();
    }
}