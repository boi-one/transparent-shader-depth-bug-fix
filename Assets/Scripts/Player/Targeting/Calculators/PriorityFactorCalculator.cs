using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityFactorCalculator : MonoBehaviour
{
    public virtual float CalculateWeight(Selectable selectable, float factorWeight)
    {
        return 0;
    }
}