using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

public class ComboSystem : MonoBehaviour
{
    // Combo
    [SerializeField] private Combo combo;
    public int comboStepIndex;
    public ComboAttack currentComboAttack;

    private HitSystem _hitSystem;

    // Animations
    [SerializeField] private Animator animator;
    private AnimationManager _animationManager;
    private AnimatorOverrideController _overrider;
    
    [SerializeField] private string[] _states = { "Male_standard_combo_swordslash1", "Male_standard_combo_swordslash2" };
    private int _currentStateId;

    // Booleans
    public bool InCombo
    {
        get => _inCombo;
        set => _inCombo = value;
    }
    private bool _maySwap = true;
    private bool _hasSwapped;

    // Unity Events
    [SerializeField] private UnityEvent comboFailed = new UnityEvent();
    [SerializeField] private UnityEvent comboSuccessful = new UnityEvent();
    [SerializeField] private UnityEvent comboStepStart = new UnityEvent();
    [SerializeField] private UnityEvent comboStepFinish = new UnityEvent();
    private bool _inCombo;

    private EntityStats _entityStats;
    private ForceBody _forceBody;

    private void Awake()
    {
        _overrider = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = _overrider;
        _animationManager = GetComponentInChildren<AnimationManager>();
        _hitSystem = GetComponent<HitSystem>();
        currentComboAttack = combo.attacks[0];
        _entityStats = GetComponent<EntityStats>();
        _forceBody = GetComponent<ForceBody>();
    }

    public void AnimateNextAttack()
    {
        if (!_maySwap || IsLastStep) return;
        
        SwitchAttack(combo.attacks[comboStepIndex]);
        comboStepIndex++;
        _maySwap = false;
        
    }

    private void SwitchAttack(ComboAttack attack)
    {
        currentComboAttack = attack;
        _hitSystem.CurrentHitboxNumber = currentComboAttack.currentHitboxNumber;
        
        var currentState = _states[_currentStateId];
        _overrider[currentState] = attack.animation;

        _currentStateId = _currentStateId == 0 ? 1 : 0;
        _hasSwapped = true;
    }

    public void SwapCombo(Combo stats) => combo = stats;

    public void InitializeStep()
    {
        if (IsFirstStep) _animationManager.ResetTrigger("comboEnded");
        
        InCombo = true;
        _maySwap = true;
        _hasSwapped = false;
        comboStepStart?.Invoke(); 
    }

    public void ExitStep()
    {
        comboStepFinish?.Invoke();
        if (_hasSwapped) return;

        if (IsLastStep) comboSuccessful?.Invoke();
        else comboFailed?.Invoke();
        
        ExitCombo();
    }

    // First attack and last attack booleans
    public bool IsLastStep => comboStepIndex >= combo.attacks.Length;

    public bool IsFirstStep => comboStepIndex == 0;
    

    public void ExitCombo()
    {
        _animationManager.ForceTrigger("comboEnded");
        _animationManager.ResetTrigger("Attack");
        _currentStateId = 0;
        comboStepIndex = 0;
        InCombo = false;
    }

    // Resets the combo when you stop walking
    public void ResetCombo()
    {
        _animationManager.ResetTrigger("Attack");
        _animationManager.ResetTrigger("comboEnded");
        InCombo = false;
    }

    public void ResetComboEnded()
    {
        _animationManager.ResetTrigger("comboEnded");
    }

    public void AddKnockbackToTarget(GameObject target)
    {
        var targetStateMachine = target.GetComponent<StateMachine.StateMachine>();
        var currentComboStep = currentComboAttack;

        if (currentComboStep.triggersHitState) targetStateMachine.SetTrigger("isHit", true);
                                                                                                                                        
        var forceIntensity = (_entityStats.Strength * _forceBody.Mass) * currentComboStep.forceIntensity;
        KnockbackSystem.Instance.AddKnockback(target,  gameObject.GetDirection(), KnockbackIds.MeleeAttack, forceIntensity, currentComboStep.forceDirection, gameObject.transform);
    }
}