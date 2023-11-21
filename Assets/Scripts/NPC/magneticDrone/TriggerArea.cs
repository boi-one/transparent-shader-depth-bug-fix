using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] private UnityEvent pushPullForce = new UnityEvent();
    //[SerializeField] private UnityEvent OnUseAbility = new UnityEvent();
    [SerializeField] private UnityEvent stopUseAbility = new UnityEvent();

    [SerializeField] private float triggerCooldown;
    [SerializeField] private float triggerRadius;
    [SerializeField] private LayerMask layer;

    private Collider[] _hitColliders = new Collider[6];

    private bool HasActivated { get; set; }

    void Start()
    {
        HasActivated = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOverlap();
    }

    public void CheckOverlap()
    {
        //Creates a detection sphere to see if the player in range is
        if (Physics.OverlapSphereNonAlloc(transform.position, triggerRadius, _hitColliders, layer) > 0 && HasActivated)
        {
            OnOverlap();
        }
    }

    private void OnOverlap()
    {
        HasActivated = false;
            
        pushPullForce.Invoke();
        StartCoroutine(Cooldown());
        stopUseAbility.Invoke();
    }
    
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(triggerCooldown);
        HasActivated = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, triggerRadius);
    }
}
