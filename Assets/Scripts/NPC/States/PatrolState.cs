using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class PatrolState : PathfindingState
{
    [SerializeField] private bool loopWaypoints;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    private int _currentWaypointIndex = -1;

    [SerializeField] private float minDistance = 15f;
    [SerializeField] private UnityEvent onPlayerFound = new UnityEvent();
    
    private void Start()
    {
        if (waypoints.Count == 0)
        {
            waypoints.Add(transform);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.CurrentTarget = GetCurrentTarget();
        EnemyAI.StartPath();
    }

    private void Update()
    {
        if (CurrentPlayer == null)
        {
            FindNearestPlayer(minDistance);
            return;
        }

        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, CurrentPlayer.transform.position) > minDistance) return;

        SetTrigger("foundPlayer");
        onPlayerFound?.Invoke();
    }

    public override void OnPathComplete(Path path)
    {
        _currentWaypointIndex++;

        if (_currentWaypointIndex > waypoints.Count - 1)
        {
            _currentWaypointIndex = 0;
            if (!loopWaypoints) waypoints.Reverse();
        }

        EnemyAI.CurrentTarget = GetCurrentTarget();
    }

    public override Vector3 GetCurrentTarget()
    {
        var waypointIndex = Math.Max(0, _currentWaypointIndex);
        
        return waypoints[waypointIndex].position;
    }
}