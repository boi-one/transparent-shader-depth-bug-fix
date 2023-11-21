using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public abstract class PathfindingState : StateBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private StateMachine.StateMachine stateMachine;
    
    private GameObject _currentPlayer;
    public GameObject CurrentPlayer
    {
        get { return _currentPlayer; }
        set
        {

            if (_currentPlayer != null)
            {
                value.GetComponent<DestroyEvent>().onDestroyed.RemoveListener(OnPlayerDestroyed);
            }
            _currentPlayer = value;
            if (_currentPlayer != null)
            {
                _currentPlayer.GetComponent<DestroyEvent>().onDestroyed.AddListener(OnPlayerDestroyed);
            }
        }
    }

    public virtual void OnPlayerDestroyed()
    {

    }

    public virtual void OnPathComplete(Path path)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.PausePath = false;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void SetTrigger(string key, bool value = true)
    {
        StateMachine.SetTrigger(key, value);
    }

    public GameObject FindNearestPlayer(float minDistance = 0)
    {
        _currentPlayer = EnemyAI.GetClosestTarget(minDistance);
        return _currentPlayer;
    }

    public virtual Vector3 GetCurrentTarget()
    {
        return transform.position;
    }

    public EnemyAI EnemyAI => enemyAI;
    public StateMachine.StateMachine StateMachine => stateMachine;
}
