using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.Profiling;

public class MagneticPlatformDetectionState : StateBehaviour

{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private MagneticPlatform magneticPlatform;

    private void FixedUpdate()
    {
        GetNearestEnemy(magneticPlatform.Radius);
    }



    private void GetNearestEnemy(float radius)
    {
        var detectionPosition = magneticPlatform.DetectionPoint.transform.position;
        Collider[] inRangeColliders = Physics.OverlapSphere(detectionPosition, radius, targetLayerMask);
        if (inRangeColliders.Length == 0) return;
        inRangeColliders.Where(c => magneticPlatform.IsTargetInRange(c.transform.position)).Where(IsAttachable).ToArray();

        var closestCollider = inRangeColliders.OrderBy(t => Vector3.Distance(detectionPosition, t.transform.position)).FirstOrDefault()!;
        magneticPlatform.Lockable = closestCollider.GetComponent<ILockable>();
        magneticPlatform.SetStateTrigger("detectedLockTarget", true);
    }

    private bool IsAttachable(Collider target)
    {
        var iTarget = target.GetComponent<ILockable>();
        return iTarget != null;
    }
}