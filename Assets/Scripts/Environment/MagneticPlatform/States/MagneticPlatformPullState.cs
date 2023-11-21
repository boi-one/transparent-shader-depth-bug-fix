using UnityEngine;

public class MagneticPlatformPullState : StateBehaviour
{
    [SerializeField] private MagneticPlatform magneticPlatform;
    [SerializeField] private string impactForceName;

    public override void OnEnter()
    {
        base.OnEnter();
        if (!magneticPlatform.IsTargetViable()) return;
        magneticPlatform.Lockable.StateTrigger("isLocked", true);
        
        magneticPlatform.Lockable.RotationTarget = magneticPlatform.LookTransform;
        
        PullTarget(magneticPlatform.Lockable.ForceBody);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void OnPulled()
    {
        string triggerName = "pulledLockTarget";
        magneticPlatform.SetStateTrigger(triggerName, true);
    }

    private void PullTarget(ForceBody targetForceBody)
    {
        var pullForce = ForceLibrary.Get(impactForceName);
        var direction = transform.position - magneticPlatform.Lockable.Transform.position;
        pullForce.Direction = direction;
        
        targetForceBody.Add(pullForce, new CallbackConfig()
        {
            OnFinish = PullFinished,
            OnCancel = PullCancelled
        });
    }

    private void PullCancelled()
    {
        magneticPlatform.SetStateTrigger("turnOff", true);
    }

    private void PullFinished()
    {
        OnPulled();
    }

}