using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TargetSelector))]
[RequireComponent(typeof(ForceBody))]
[RequireComponent(typeof(PlayerMovement))]


public class PullAbility : Ability
{
    [SerializeField] private float isPullingResetDelay = 0.5f;
    [SerializeField] private float pullSpeed = 1.0f;
    [SerializeField] private Force pullForce;
    [SerializeField] private Force freezeForce;
    [SerializeField] private Force residualForce;
    
    [SerializeField] private UnityEvent onPull = new UnityEvent();
    [SerializeField] private UnityEvent onFreeze = new UnityEvent();
    [SerializeField] private UnityEvent onLaunch = new UnityEvent();
    [SerializeField] private UnityEvent onLaunchEnd = new UnityEvent();

    private TargetSelector _targetSelector;
    private ForceBody _forceBody;
    private PlayerMovement _playerMovement;
    private float _magnitude;
   
    private Transform _rotationPosition = null;

    private Selectable _selected;
    
    [SerializeField] private string impactForceName = "Pull";

    public bool IsPulling { get; set; }

    private void Awake()
    {
        _forceBody = GetComponent<ForceBody>();
        _targetSelector = GetComponent<TargetSelector>();
        _playerMovement = GetComponent<PlayerMovement>();
        _magnitude = residualForce.Direction.magnitude;
    }

    private void Start()
    {
        _playerMovement.GetGroundChangeEvent.AddListener(isGrounded =>
        {
            if (!isGrounded) return;
        });
        Init();
    }

    public override bool Use(InputAction.CallbackContext context)
    {
        _selected = _targetSelector.GetSelected();
        if (_targetSelector == null || _selected == null) return false;
        
        if (!CanUseAbility) return true;

        EmitUse();

        var direction = _selected.GetPosition() - transform.position;
        pullForce.Direction = direction;
        pullForce.Duration = direction.magnitude / pullSpeed;

        _forceBody.Add(pullForce, new CallbackConfig()
        {
            OnFinish = AddFreezeForce,
            OnCancel = AddFreezeForce
        });

        onPull.Invoke();
        
        gameObject.LookAt(direction);

        CanUseAbility = false;
        IsBlockingMovement = true;
        IsPulling = true;
        
        StartResetDelay();
        return true;
    }

    private void PullObject(GameObject targetGameObject)
    {
        var forceBody = targetGameObject.GetComponent<ForceBody>();
        if (forceBody == null) return;
        
        var impactForce = ForceLibrary.Get(impactForceName);
        var impactDirection = pullForce.Direction.normalized * impactForce.Direction.magnitude;
        impactDirection *= -1;
        impactForce.Direction = impactDirection;
        forceBody.Add(impactForce);
    }

    private void AddFreezeForce()
    {
        onFreeze.Invoke();
        _forceBody.Add(freezeForce, new CallbackConfig()
        {
            OnFinish = AddResidualForce,
            OnCancel = Reset
        });
    }

    private void AddResidualForce()
    {
        onLaunch.Invoke();
        IsBlockingMovement = false;
        
        if(_selected == null) return;
        var target = _selected.TargetGameObject;
        if(target != null) PullObject(target);
        
        Reset();
        
        residualForce.Direction = pullForce.Direction;
        residualForce.Direction = residualForce.Direction.normalized * _magnitude;
        _forceBody.Add(residualForce, new CallbackConfig()
        {
            OnFinish = () => onLaunchEnd.Invoke()
        });
    }

    public override void Reset()
    {
        base.Reset();
        
        transform.rotation = Quaternion.identity;
        _targetSelector.Deselect();
    }

    public void StartResetDelay()
    {
        StartCoroutine(Delay());
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(isPullingResetDelay);
        IsPulling = false;
    }

    public override void OnInput(InputAction.CallbackContext aContext)
    {
        Use(aContext);
    }
}