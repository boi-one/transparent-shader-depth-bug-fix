using System;
using UnityEngine;
using UnityEngine.Events;

public class FollowState : PathfindingState
{
    [SerializeField] private float maxDistance = 25f;
    
    [SerializeField] private float rangeDistance = 5f;
    
    [SerializeField] private UnityEvent onPlayerRangeEnter = new UnityEvent();
    [SerializeField] private UnityEvent onOutofRange = new UnityEvent();
    [SerializeField] private Vector3 _followOffset = Vector3.zero;
    void Start()
    {
        // todo: waarom staat dit er nog in? Lijkt me niet nodig (en niet zo wenselijk eigenlijk)
        InvokeRepeating(nameof(UpdatePath), 0f, 0f);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        EnemyAI.CurrentTarget = GetCurrentTarget();
        EnemyAI.StartPath();
    }

    private void FixedUpdate()
    {
        if (CurrentPlayer == null) return;

        float distance = Vector3.Distance(transform.position, CurrentPlayer.transform.position);

        if (distance >= maxDistance)
        {
            SetTrigger("outofRange");
            onOutofRange?.Invoke();
        }

        if (distance <= rangeDistance)
        {
            onPlayerRangeEnter?.Invoke();
        }
        EnemyAI.CurrentTarget = GetCurrentTarget();
    }
    
    public override Vector3 GetCurrentTarget()
    {
        if (GameObject.FindWithTag("Player") == null) return Vector3.zero;

        FindNearestPlayer(maxDistance);

        if (CurrentPlayer == null) return Vector3.zero;

        return CurrentPlayer.transform.position + _followOffset;
    }
    
    private void UpdatePath()
    {
        if (EnemyAI.Seeker.IsDone())
        {
            EnemyAI.StartPath();
        }
    }
}
