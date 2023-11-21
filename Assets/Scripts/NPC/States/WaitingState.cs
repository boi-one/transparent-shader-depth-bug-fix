using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitingState : PathfindingState
{
    [SerializeField] private UnityEvent onWait = new UnityEvent();
 
    [SerializeField] private int counter;
    [SerializeField] private int maxCounter;
    [SerializeField] private float delayTime;

    public override void OnEnter()
    {
        base.OnEnter();
        StartCoroutine(Delay());
    }
    private IEnumerator Delay()
    {
        counter += 1;
        if (counter >= maxCounter)
        {
            onWait?.Invoke();
            //EnemyAI.PausePath = true;
            yield return new WaitForSeconds(delayTime);
            counter = 0;
        }
        
        SetTrigger("hasWaited");
    }
}
