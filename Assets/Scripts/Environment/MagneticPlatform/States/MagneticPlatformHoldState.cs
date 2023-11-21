using UnityEngine;

public class MagneticPlatformHoldState : StateBehaviour
{
    [SerializeField] private MagneticPlatform magneticPlatform;
    private Force _lockForce;

    public override void OnEnter()
    {
        base.OnEnter();
        if (!magneticPlatform.IsTargetViable()) return;
        AddLockForce();
        magneticPlatform.Lockable.OnLockRelease.AddListener(ReleaseLock);
    }

    private void AddLockForce()
    {
        _lockForce = ForceLibrary.Get("Lock");

        magneticPlatform.Lockable.ForceBody.Add(_lockForce);
    }

    public override void OnExit()
    {
        base.OnExit();
        magneticPlatform.Lockable.ForceBody.Remove(_lockForce.Id);
        magneticPlatform.Lockable.OnLockRelease.RemoveListener(ReleaseLock);
    }

    private void FixedUpdate()
    {
        magneticPlatform.IsTargetViable();
    }

    private void ReleaseLock()
    {
        magneticPlatform.Release(true);
    }
    
}