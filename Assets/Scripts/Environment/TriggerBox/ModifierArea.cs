using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierArea : TriggerBox
{
    [SerializeField] private int index;

    public override void OnRangeEnter(GameObject other)
    {
        if (other.TryGetComponent<IModifiable>(out var modifiable))
        {
            modifiable.Modify(index);
            
            base.OnRangeEnter(other);
        }
    }
}