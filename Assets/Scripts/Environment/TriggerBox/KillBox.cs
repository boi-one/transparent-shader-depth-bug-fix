using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : TriggerBox
{
    public override void OnRangeEnter(GameObject other)
    {
        base.OnRangeEnter(other);

        other.Kill();
    }
}
