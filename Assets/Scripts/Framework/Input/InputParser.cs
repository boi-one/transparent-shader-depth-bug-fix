using System;
using Skills.Grabbing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class InputParser : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputActionAsset _playerControlsActions;

    [Header("Scripts")] 
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TargetSelector targetSelector;
    [SerializeField] private GrabSkill grabSkill;
    private StaminaLookup _staminaLookup;

    [Header("Class Specific Scripts")]
    [SerializeField] private ComboSystem comboSystem;
    [SerializeField] private Ability playerBasicAbility;
    [SerializeField] private Ability playerSpecialAbility;
    
    [Header("Optional/Temp Scripts")] //als je een script toevoeg in deze header vergeet niet om een null check te maken.
    [SerializeField] private LevelReloader levelReloader;
    
    [Header("Booleans")]
    [SerializeField] private bool canMoveWhileAiming = true;

    public bool CannotMove
    {
        get => _cannotMove;
        set => _cannotMove = value;
    }

    private bool _maySprint = true;

    [Header("Strings")] 
    [SerializeField] private string specialAbilityName;

    private bool _cannotMove;

    /// <summary>
    /// Instructions:
    ///
    /// To bind a function, it needs to meet the following parameters.
    /// - The function needs to be public
    /// - The function needs to have an Argument of the type InputAction.CallbackContext (named context for convenience)
    ///
    /// Please add a reference to your script under the header Scripts so we have clear references.
    /// </summary>
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerControlsActions = _playerInput.actions;

        _staminaLookup = GetComponent<StaminaLookup>();

        levelReloader = FindObjectOfType<LevelReloader>();

        _playerControlsActions["Jump"].performed += Jump;
        _playerControlsActions["Grab"].performed += Grab;

        _playerControlsActions["ReleaseGrab"].started += ReleaseGrab;
        _playerControlsActions["ClimbUp"].started += ClimbUp;
        _playerControlsActions["ResetLevel"].started += ResetLevel;

        InstantiatePlayerAbility();
        if (targetSelector != null) TargetingStart();

        _playerControlsActions.Enable();
    }

    private void InstantiatePlayerAbility()
    {
        if (playerSpecialAbility != null)
        {
            _playerControlsActions["SpecialAttack"].performed += SpecialAttack;
            _playerControlsActions["SpecialAttack"].canceled += playerSpecialAbility.StopUse;
        }
        if (playerBasicAbility != null)
        {
            _playerControlsActions["BaseAttack"].performed += BaseAttack;
        }
    }

    private void FixedUpdate()
    {
        if (targetSelector != null) TargetingStart(false);
        
        var isBlockingMovement = CannotMove || playerSpecialAbility != null && playerSpecialAbility.IsBlockingMovement;
        if (isBlockingMovement)
        {
            if(CannotMove)playerMovement.currentMovementState = MovementStates.Walking; // todo: dit moet wat netter gemaakt worden.
            return;
        }

        Vector2 moveInput = _playerControlsActions["Move"].ReadValue<Vector2>();
        var isSprinting = _playerControlsActions["SprintActivator"].ReadValue<float>() == 1 && moveInput.magnitude != 0;

        var canSprint = isSprinting && _staminaLookup.UseStamina("Sprinting");
        var canGrab = grabSkill.IsGrabbing && _staminaLookup.UseStamina("Grabbing");
        MovementStates currentMovementState;

        if (canSprint && _maySprint) currentMovementState = MovementStates.Sprinting;
        else if (!_maySprint) currentMovementState = MovementStates.Exhausted;
        else currentMovementState = MovementStates.Walking;

        playerMovement.currentMovementState = currentMovementState;
        if (comboSystem == null || !comboSystem.InCombo || !CannotMove) playerMovement.Move(moveInput);
    }

    public void ToggleSprint(bool isAllowed)
    {
        _maySprint = isAllowed;
    }

    public InputActionAsset PlayerControlsActions => _playerControlsActions;

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void RemoveListeners()
    {
        _playerControlsActions["Jump"].performed -= Jump;
        _playerControlsActions["Grab"].performed -= Grab;

        _playerControlsActions["ReleaseGrab"].started -= ReleaseGrab;
        _playerControlsActions["ClimbUp"].started -= ClimbUp;
        _playerControlsActions["ResetLevel"].started -= ResetLevel;
        
        TargetingCancelled();

        _playerControlsActions["ControllerAim"].performed -= targetSelector.ControllerMove;
        _playerControlsActions["MousePosition"].performed -= targetSelector.MouseMove;

        if (playerSpecialAbility != null)
        {
            _playerControlsActions["SpecialAttack"].performed -= SpecialAttack;
            _playerControlsActions["SpecialAttack"].canceled -= playerSpecialAbility.StopUse;
        }

        if (playerBasicAbility != null)  _playerControlsActions["BaseAttack"].performed -= BaseAttack;
    }
    
    private void Jump(InputAction.CallbackContext context) => _staminaLookup.UseStamina("Jumping", playerMovement.JumpPrimer, context);
    private void Grab(InputAction.CallbackContext context) => _staminaLookup.UseStamina("Grabbing", grabSkill.OnGrabButtonPressed, context);
    private void ReleaseGrab(InputAction.CallbackContext context) => grabSkill.ForceRelease(true);
    private void ClimbUp(InputAction.CallbackContext context) => grabSkill.ForceRelease();
    private void ResetLevel(InputAction.CallbackContext context) => levelReloader.Reset();
    
    private void TargetingCancelled()
    {
        targetSelector.CanAimFreely = false;
        CannotMove = false;
        _playerControlsActions["ControllerAim"].performed -= targetSelector.ControllerMove;
        _playerControlsActions["MousePosition"].performed -= targetSelector.MouseMove;
    }

    private void TargetingStart(bool isStart = true)
    {
        targetSelector.Activate();
        if (isStart)
        {
            targetSelector.CanAimFreely = true;
            _playerControlsActions["ControllerAim"].performed += targetSelector.ControllerMove;
            _playerControlsActions["MousePosition"].performed += targetSelector.MouseMove;
        }
    }
    
    private void SpecialAttack(InputAction.CallbackContext context)
    {
        if (_staminaLookup.ValidateAction(specialAbilityName))
        {
            if (playerSpecialAbility.Use(context)) _staminaLookup.DepleteStamina(specialAbilityName);
            _staminaLookup.UseStamina(specialAbilityName, playerSpecialAbility.Use, context);
        }
    }
    
    private void BaseAttack(InputAction.CallbackContext context)
    {
        _staminaLookup.UseStamina("Base Attack", playerBasicAbility.Use, context);
    }
}