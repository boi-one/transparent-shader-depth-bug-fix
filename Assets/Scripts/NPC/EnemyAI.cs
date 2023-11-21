using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

public class EnemyAI : StateBehaviour
{
    [SerializeField] private float speed = 500f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1, 0);
    [SerializeField] private float nextWaypointDistance = 1f;
    [SerializeField] private UnityEvent<Path> onPathFound = new UnityEvent<Path>();
    public UnityEvent onPathCompleted = new UnityEvent();

    private float _defaultSpeed;
    private Vector3 _currentTarget;
    private Path _path;
    private int _pathIndex = 0;
    private bool _reachedEndOfPath = false;
    private bool _pausePath;
    private Vector3 _currentVelocity;
    
    [SerializeField] public Force movementForce;

    public bool PausePath
    {
        get => _pausePath;
        set => _pausePath = value;
    }
    private Seeker _seeker;

    public Seeker Seeker => _seeker;
    public float DefaultSpeed => _defaultSpeed;

    [SerializeField] public ForceBody forceBody;
    
    public Vector3 CurrentTarget
    {
        set
        {
            value += targetOffset;
            _currentTarget = value;
        }
        
    }
    public bool Enabled { get; set; } = true;
    
    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    
    public virtual void Start()
    {
        _seeker = gameObject.GetOrAddComponent<Seeker>();
        _defaultSpeed = speed;
        StartPath();
    }

    private void OnPathComplete(Path path)
    {
        if (path.error) return;
        
        _path = path;
        _pathIndex = 0;
        onPathFound?.Invoke(path);
        // todo: is dit hier nodig? Lijkt wat hard gekoppeld. Zou beter de currentState.OnPathComplete aan kunnen roepen vanuit event
        if (GetComponent<PatrolState>().enabled) {
            GetComponent<PatrolState>().OnPathComplete(path);
        }
    }
    
    public virtual void FixedUpdate()
    {
        if (!Enabled || _path == null) return;
        if (_pathIndex >= _path.vectorPath.Count && !_reachedEndOfPath)
        {
            _reachedEndOfPath = true;
            onPathCompleted?.Invoke();
            StartPath();
            return;
        }
        
        if (_pathIndex >= _path.vectorPath.Count) return;
        if (_pausePath) return;

        _reachedEndOfPath = false;
        
        Vector3 currentWaypoint = _path.vectorPath[_pathIndex];

        _currentVelocity = SteerToTarget(currentWaypoint);
        movementForce.Direction = _currentVelocity;
        forceBody.Add(movementForce);

        float distance = Vector3.Distance(transform.position, currentWaypoint);
        if (distance < nextWaypointDistance) NextPathIndex();
       TargetLookAt(_currentVelocity);
    }

    private void NextPathIndex()
    {
        _pathIndex++;
    }

    public void ResetMovement()
    {
        movementForce.Direction = Vector3.zero;
    }
    
    public void StopMovement()
    {
        forceBody.Remove(movementForce.Id);
    }

    public virtual Vector3 SteerToTarget(Vector3 currentWaypoint)
    {
        var position = transform.position;
        Vector3 direction = currentWaypoint - position;
        cw = currentWaypoint;
        Debug.DrawLine(position, currentWaypoint);
        direction.Normalize();
        
        Vector3 force = direction * Speed * Time.fixedDeltaTime;

        var steeringForce = force - movementForce.Direction;
        var newVelocity = movementForce.Direction + (steeringForce / mass);
        newVelocity /= Time.fixedDeltaTime;
        return newVelocity;
    }

    private Vector3 cw;
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cw, 1);
    }

    public void StartPath()
    {
        if (_currentTarget == Vector3.zero || _seeker == null) return;
        
        _seeker.StartPath(transform.position, _currentTarget, OnPathComplete);
    }

    public GameObject GetClosestTarget(float length = 0)
    {
        GameObject currentPlayer = null;

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").OrderBy(player => Vector3.Distance(transform.position, player.transform.position)).ToList();

        // todo: uitbreiden met of de player zichtbaar is
        if (players.Count > 0) currentPlayer = players[0];

        return currentPlayer;
    }
    
    private bool IsHitValid(GameObject player, RaycastHit hit)
    {
        return hit.collider != null && hit.collider.gameObject == player;
    }
    public void TargetLookAt(Vector2 targetPos)
    {
        Vector3 localScale = transform.localScale;
        if (targetPos.magnitude < 1) return;

        var direction = Mathf.Sign(targetPos.x) * Math.Abs(localScale.x);
        transform.localScale = new Vector3(direction, localScale.y, localScale.z);
    }

    public void LookAtClosestTarget()
    {
        var target = GetClosestTarget();
        TargetLookAt(target.transform.position);
    }

    public void ResetSpeed()
    {
        Speed = _defaultSpeed;
    }
}
