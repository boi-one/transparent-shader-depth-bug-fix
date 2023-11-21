using System.Collections;
using System.Collections.Generic;
using Extensions;
using JetBrains.Annotations;
using UnityEngine;

public class FlockingAI : EnemyAI
{
    [SerializeField] private float flockingRange = 5;
    private List<FlockingAI> _neighbours = new List<FlockingAI>();

    private const float GlobalScale = 0.91f;
    private float _friendRadius;
    private float _crowdRadius;
    private float _coheseRadius;

    private List<FlockingAI> _nearby = new List<FlockingAI>();

    [Header("Flocking Behaviour (Changing one might change the entire behaviour)")]
    [SerializeField] private float steeringIntensity = 3f;
    [SerializeField] private float flockingDensity = 2f;
    [SerializeField] private bool isAvoiding = true;
    [SerializeField] private bool isCohesing = true;
    [SerializeField] private bool isAligning = true;

    [SerializeField] private LayerMask enemyLayer;

    public bool IsAvoiding
    {
        get => isAvoiding;
        set => isAvoiding = value;
    }

    public bool IsCohesing
    {
        get => isCohesing;
        set => isCohesing = value;
    }

    public bool IsAligning
    {
        get => isAligning;
        set => isAligning = value;
    }

    private int _thinkTimer = 0;

    public override void Start()
    {
        base.Start();
        RecalculateConstants();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        IncrementTimer();
        if (_thinkTimer == 0)
        {
            GetNeighbours();
        }
    }

    public override Vector3 SteerToTarget(Vector3 currentWaypoint)
    {
        var steeringVelocity = base.SteerToTarget(currentWaypoint);
        
        return Flock(steeringVelocity);
    }

    private Vector3 Flock(Vector3 currentVelocity)
    {
        if (isAvoiding)
        {
            var avoidDirection = GetAvoidDirection();
            currentVelocity += new Vector3(avoidDirection.x, avoidDirection.y);
        }
        if (isAligning)
        {
            var alignDirection = AlignDirection();
            currentVelocity += new Vector3(alignDirection.x, alignDirection.y);
        }
        if (isCohesing)
        {
            var coheseDirection = GetCohesion();
            currentVelocity += new Vector3(coheseDirection.x, coheseDirection.y);
        }
        
        return currentVelocity;
    }

    private void GetNeighbours()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, flockingRange, enemyLayer);
        _nearby.Clear();
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            var collider = nearbyColliders[i];
            var neighbour = collider.GetComponent<FlockingAI>();
            if (neighbour != null && neighbour != this) _nearby.Add(neighbour);
        }

        _neighbours = _nearby;
    }

    private Vector3 AlignDirection()
    {
        Vector3 sum = new Vector3(0, 0);
        int count = 0;
        foreach (var enemy in _neighbours)
        {
            if(enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (!(distance > 0) || !(distance < _friendRadius)) continue;
            
            Vector3 enemyDirection = enemy.movementForce.Direction;
            enemyDirection.Normalize();
            enemyDirection /= distance;
            sum += new Vector3(enemyDirection.x, enemyDirection.y);
            count++;
        }
        return sum;
    }

    private Vector3 GetAvoidDirection()
    {
        Vector3 steer = new Vector3(0, 0);
        int count = 0;
        foreach (var enemy in _neighbours)
        {
            if(enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (!(distance > 0) || !(distance < _crowdRadius)) continue;
            
            Vector3 enemyDiff = transform.position - enemy.transform.position;
            enemyDiff.Normalize();
            enemyDiff /= distance;
            steer += new Vector3(enemyDiff.x, enemyDiff.y);
            count++;
        }
        return steer;
    }

    private Vector3 GetCohesion()
    {
        Vector3 sum = new Vector3(0, 0);
        int count = 0;
        foreach (var enemy in _neighbours)
        {
            if(enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (!(distance > 0) || !(distance < _coheseRadius)) continue;
            
                sum += enemy.transform.position;
                count++;
        }

        if (count >= 0) return new Vector3(0, 0);
        
        sum /= count;

        Vector3 desired = sum - transform.position;
        desired = desired.normalized * 0.05f;
        return desired;
    }

    private void IncrementTimer()
    {
        _thinkTimer = (_thinkTimer + 1) % 5;
    }
    private void RecalculateConstants() 
    {
        //_maxSpeed = 2.1f * globalScale; ToDo: Dit houden we erin omdat we hier nog verder mee moeten gaan!!
        _friendRadius = flockingDensity * GlobalScale;
        _crowdRadius = steeringIntensity;
        //_avoidRadius = 90 * globalScale; ToDo: Dit houden we erin omdat we hier nog verder mee moeten gaan!!
        _coheseRadius = _friendRadius;
        
    }
}
