using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AnimationManager animationManager;

    [SerializeField] private float specialWaitTime = 0.5f;
    
    private Animator _animator;

    private static readonly int DrawWeapon = Animator.StringToHash("DrawWeapon");
    private static readonly int IsWeaponDrawn = Animator.StringToHash("IsWeaponDrawn");
    
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private static readonly int Special = Animator.StringToHash("Special");
    private static readonly int IsSpecialActive = Animator.StringToHash("IsSpecialActive");

    private Coroutine _resetCoroutine;
    private int _gunLayerIndex;
    private int _swordLayerIndex;

    private const float IdleStateDelay = 6f;
    private const float IdleStateTransitionSpeed = 1f;

    private void Awake()
    {
        _animator = animationManager.Animator;
        _gunLayerIndex = _animator.GetLayerIndex("Gun layer");
        _swordLayerIndex = _animator.GetLayerIndex("Sword layer");
    }

    public Animator Animator => _animator;
    public void SelectSword() => SelectWeapon(_swordLayerIndex);
    public void SelectGun() => SelectWeapon(_gunLayerIndex);
    private void SelectWeapon(int layerIndex)
    {
        _animator.SetLayerWeight(layerIndex, 1f);
        _animator.SetBool(IsWeaponDrawn, true);
        if (playerMovement.IsGrounded) animationManager.SetTrigger(DrawWeapon);
        StartResetCoroutine();
    }

    public void DoSpecial(int layerIndex)
    {
        _animator.SetLayerWeight(layerIndex, 1f);
        _animator.SetBool(IsSpecialActive, true);
        _animator.SetTrigger(Special);
        StartCoroutine(WaitForTime());
        StartResetCoroutine();
    }

    public void CancelSpecial()
    {
        _animator.SetBool(IsSpecialActive, false);
    }

    public void DoAttack()
    {
        StartResetCoroutine();
    }

    private void StartResetCoroutine()
    {
        if (_resetCoroutine != null) StopCoroutine(_resetCoroutine);
        _resetCoroutine = StartCoroutine(ResetAttackState());
    }

    public IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(IdleStateDelay);

        float percentage = 0f;
        while (percentage < 1)
        {
            if (percentage > 1) percentage = 1;
            percentage += Time.deltaTime * IdleStateTransitionSpeed;
            _animator.SetLayerWeight(_gunLayerIndex, Mathf.Lerp(_animator.GetLayerWeight(_gunLayerIndex), 0, percentage));        
            _animator.SetLayerWeight(_swordLayerIndex, Mathf.Lerp(_animator.GetLayerWeight(_swordLayerIndex), 0, percentage));
            yield return null;
        }
    }

    public IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(specialWaitTime);
        CancelSpecial();
        _animator.SetBool(IsWeaponDrawn, false);
    }
}
