using UnityEngine;

public class ComboStateChecker : FreezeFrameState
{
    private Animator _animator;
    private bool _isExitTriggered;
    private ComboSystem _comboSystem;
    private AnimationManager _animationManager;
    private HitSystem _hitSystem;
    private ForceBody _forceBody;
    private float _defaultGravityScale;
    private Transform _playerTransform;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _forceBody = animator.GetComponentInParent<ForceBody>();
        _playerTransform = _forceBody.GetComponent<Transform>();
        _comboSystem = animator.GetComponentInParent<ComboSystem>();
        _animationManager = animator.GetComponentInParent<AnimationManager>();
        _hitSystem = animator.GetComponentInParent<HitSystem>();
        _comboSystem.InitializeStep();
        if(_comboSystem.currentComboAttack.useParticle) 
            ParticleManager.Instance.PlayParticle(_comboSystem.currentComboAttack.particleID, _playerTransform);
        _animator.speed = _comboSystem.currentComboAttack.animationSpeed;
        _defaultGravityScale = _forceBody.GravityScale;
        _isExitTriggered = false;


        
        isStateUncancellable = _comboSystem.currentComboAttack.isStateUncancellable;
        freezeTiming = _comboSystem.currentComboAttack.freezeTiming;
        freezeTime = _comboSystem.currentComboAttack.freezeTime;
        frameId = _comboSystem.currentComboAttack.frameId;
        
        var currentAttack = _comboSystem.currentComboAttack;

        if (currentAttack.inAir) _forceBody.GravityScale = currentAttack.gravityScale;
        _comboSystem.ResetComboEnded();
        
        base.OnStateEnter(animator, stateInfo, layerIndex);


    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (stateInfo.normalizedTime >= 1 && !_isExitTriggered && frameId != FreezeFrameIds.LastFrame) TriggerExit();

        var normalizedTime = stateInfo.normalizedTime;
        var startActiveHitbox = _comboSystem.currentComboAttack.startActiveHitbox;
        var endActiveHitbox = _comboSystem.currentComboAttack.endActiveHitbox;
        if (normalizedTime  > startActiveHitbox ) _hitSystem.IsPerformingAttack = true;
        if (normalizedTime  > endActiveHitbox ) _hitSystem.IsPerformingAttack = false;
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _forceBody.GravityScale = _defaultGravityScale;

        if (_isExitTriggered) return;
        TriggerExit();
    }

    protected override void DoResume()
    {
        base.DoResume();
        if (frameId == FreezeFrameIds.LastFrame) TriggerExit();
    }

    public void TriggerExit()
    {
        _comboSystem.ExitStep();
        _hitSystem.IsPerformingAttack = false;
        _isExitTriggered = true;
    }
}