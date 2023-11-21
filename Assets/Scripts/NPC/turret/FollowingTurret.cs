using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

public class FollowingTurret : BaseTurret
{
    private Transform _target;
    private GameObject _player;
    [SerializeField] private float detectionRange = 100;
    private TargetRotator _targetRotator;

    [SerializeField] private float detectionAngle;
    private Vector3 _defaultTarget;
    public float DetectionRange => detectionRange;
    public float DetectionAngle => detectionAngle;

    private void Start()
    {
        CreateLaserTarget();
        turretWeapon.onStopShooting.AddListener(Unfollow);
    }

    protected void Awake()
    {
        base.Awake();
        _targetRotator = GetComponent<TargetRotator>();
    }

    private void Update()
    {
        if (IsAutoShooting) return;
        
        GetClosestTarget();
        if (!_player) return;
        
        var distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        var direction = _player.transform.position - transform.position;
        if (!(distanceToPlayer <= detectionRange)) return;
        if (!(transform.right.IsSameDirectionAs(direction.normalized, detectionAngle))) return;
        UpdateShooting();
    }

    public GameObject GetClosestTarget(float length = 0)
    {
        GameObject currentPlayer = null;

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").OrderBy(player => Vector3.Distance(transform.position, player.transform.position)).ToList();

        if (players.Count > 0) currentPlayer = players[0];

        _player = currentPlayer;
        return currentPlayer;
    }

    public void Unfollow()
    {
        _target.position = _defaultTarget;
        RotateToTarget();
    }

    private void CreateLaserTarget()
    {
        var targetObject = new GameObject();
        targetObject.name = "lasertarget";
        _target = targetObject.transform;
        _defaultTarget = transform.position + transform.right;
        _target.position = _defaultTarget;
    }

    private void RotateToTarget()
    {
        _targetRotator.Target = _target;
    }

    private void UpdateShooting()
    {
        UpdateTargetPosition();
        Follow();
        Shoot();
    }

    private void UpdateTargetPosition()
    {
        _target.position = _player.transform.position;
    }

    private void Follow()
    {
        RotateToTarget();
    }
}