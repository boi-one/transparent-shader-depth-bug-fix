using System.Linq;
using System.Net.NetworkInformation;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

public class Slingshot : MonoBehaviour
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent holdingSlingshotTarget = new UnityEvent();
    [SerializeField] private UnityEvent holdingSlingshotInAir = new UnityEvent();
    [SerializeField] private UnityEvent onSlingshot = new UnityEvent();
    
    [Header("Synergy Targets")]
    private TargetSelector[] _synergyTargets;
    private TargetSelector _currentTarget;

    [Header("Grabbing")]
    [SerializeField] private float radius = 4f;
    private bool _validTargetDetected;

    [Header("Force")]
    [SerializeField] private float distanceForce = 30f;
    private Vector3 _forceDirection = new Vector3(1, 1, 0);
    private ForceBody _forceBody;
    private PlayerMovement _playerMovement;
    [SerializeField] private Force throwForce;

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void CheckValidSynergyTargets()
    {
        var squaredRadius = radius * radius;
        var allTargets = FindObjectsOfType<TargetSelector>().ToList();
        _synergyTargets = allTargets.Where(target => target.HasComponent<PullAbility>() && (target.transform.position - GetComponentInChildren<Selectable>().transform.position).sqrMagnitude < squaredRadius && target.GetComponent<PullAbility>().IsPulling).ToArray();
        
        if (!_synergyTargets.IsEmpty()) TargetFound();
    }

    private void Update()
    {
        if (_validTargetDetected) HoldTarget();
    }

    private void TargetFound()
    {
        _currentTarget = _synergyTargets[0];
        _validTargetDetected = true;
        SetupTarget();
    }
    
    private void SetupTarget()
    {
        _currentTarget.gameObject.Hide();

        _currentTarget.GetComponent<PullAbility>().IsPulling = false;
        _forceBody = _currentTarget.GetComponent<ForceBody>();
    }

    private void HoldTarget()
    {
        _currentTarget.transform.position = transform.position;

        if (!_playerMovement.IsGrounded) holdingSlingshotInAir?.Invoke();
        else holdingSlingshotTarget?.Invoke();
    }

    public void UseSlingShot()
    {
        if (_synergyTargets.Length == 0) return;
        ShootTarget();
        onSlingshot?.Invoke();
        _validTargetDetected = false;
    }
    
    private void ShootTarget()
    {
        ApplyForce();
        
        _currentTarget.gameObject.Hide(false);
        _currentTarget.gameObject.LookAt(_forceDirection);
        
        _currentTarget.GetComponentInChildren<PlayerAnimator>().DoSpecial(2);
    }

    private void ApplyForce()
    {
        _forceBody.Remove(ForceIds.Pull);
        var force = GetForce(_forceBody.Mass);
        _forceBody.Add(force);
    }

    private Force GetForce(float mass = 1)
    {
        _forceDirection = GetComponent<TargetSelector>().CurrentDirection;
        float impact = 1;
        var impactForce = _forceDirection * impact * distanceForce;
        throwForce.Direction = impactForce / mass;
        return throwForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetComponent<BoxCollider>().bounds.center, radius);
    }
}
