using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class FreezeFrameState : StateMachineBehaviour
{
    public FreezeTiming freezeTiming = FreezeTiming.Time;
    public float freezeTime;
    public bool isStateUncancellable;
    public bool hitboxEnabled;
    public FreezeFrameIds frameId = FreezeFrameIds.LastFrame;
    private HitSystem _hitSystem;

    public FreezeFrameIds FrameId
    {
        get => frameId;
        set => frameId = value;
    }

    private bool _isFrozen;
    
    private Animator _animator;

    private IEnumerator _resumeCoroutine;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _hitSystem = _animator.GetComponentInParent<HitSystem>();
        _isFrozen = false;
        animator.SetBool("IsStateUncancellable", isStateUncancellable);
        
        if(frameId == FreezeFrameIds.FirstFrame) Freeze();
        if (hitboxEnabled) _hitSystem.IsPerformingAttack = true;
    }

    private IEnumerator Resume()
    {
        var time = freezeTiming == FreezeTiming.Time ? freezeTime : 1f / 60 * freezeTime;
        yield return new WaitForSeconds(time);
        DoResume();
    }

    protected virtual void DoResume()
    {
        if (_animator == null) return;
        _animator.speed = 1;
        _animator.SetBool("IsStateUncancellable", false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator.SetBool("IsStateUncancellable", false);
        if (_resumeCoroutine == null) return;
        
        ThreadUtility.StopDelayedTask(_resumeCoroutine);
        if (hitboxEnabled) _hitSystem.IsPerformingAttack = false;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1 && !_isFrozen && frameId == FreezeFrameIds.LastFrame) Freeze();
    }

    protected void Freeze()
    {
        _animator.speed = 0;
        _resumeCoroutine = Resume();
        ThreadUtility.StartDelayedTask(_resumeCoroutine);
        _isFrozen = true;
    }
}