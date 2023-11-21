using System.Collections;
using System.Collections.Generic;
using StateMachine.PlayerInputHandlers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class Ability : InputHandler
{
    [SerializeField] private float abilityDelay = 2.5f;
    [SerializeField] protected UnityEvent onUseAbility = new UnityEvent();
    [SerializeField] protected UnityEvent onGroundedUse = new UnityEvent();
    [SerializeField] protected UnityEvent onAiredUse = new UnityEvent();
    [SerializeField] protected UnityEvent onStopUseAbility = new UnityEvent();
    
    protected UnityEvent<bool> OnCanUseAbilityChange = new UnityEvent<bool>();

    private PlayerMovement _playerMovement;

    private bool _canUseAbility = true;
    private Coroutine _delayRoutine = null;

    void Start()
    {
        Init();
    }

    protected void Init()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public bool CanUseAbility
    {
        get => _canUseAbility;
        set
        {
            OnCanUseAbilityChange?.Invoke(value);
            _canUseAbility = value;
        }
    }

    public bool IsBlockingMovement { get; set; }

    public abstract bool Use(InputAction.CallbackContext context);

    public virtual void StopUse(InputAction.CallbackContext context)
    {
        onStopUseAbility?.Invoke();
    }

    public virtual void Reset()
    {
        IsBlockingMovement = false;
        CanUseAbility = true;
    }

    public bool IsGrounded => _playerMovement == null || _playerMovement.IsGrounded;

    protected void StartDelay()
    {
        if (_delayRoutine != null) StopCoroutine(_delayRoutine);
        _delayRoutine = StartCoroutine(AbilityDelay());
    }

    private IEnumerator AbilityDelay()
    {
        yield return new WaitForSeconds(abilityDelay);
        CanUseAbility = true;
    }

    protected void EmitUse()
    {
        onUseAbility?.Invoke();
        if (IsGrounded)
        {
            onGroundedUse?.Invoke();
        }
        else
        {
            onAiredUse?.Invoke();
        }
    }

}