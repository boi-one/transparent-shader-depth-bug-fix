using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CinamachineTargetGroupApi : CinemachineTargetGroup
{
    public void AddTarget(Transform target)
    {
        List<Target> targets = new List<Target>(m_Targets);

        Target newTarget = new Target();
        newTarget.weight = 3;
        newTarget.radius = 1;
        newTarget.target = target;
        
        targets.Add(newTarget);
        
        m_Targets = targets.ToArray();        
    }

    public void RemoveTarget(Transform target)
    {
        List<Target> targets = new List<Target>(m_Targets);
        if (targets.Count(aTarget => aTarget.target == target) == 0) return;

        targets.RemoveAt(targets.IndexOf(targets.First(aTarget => aTarget.target == target)));
        m_Targets = targets.ToArray();
    }
}
