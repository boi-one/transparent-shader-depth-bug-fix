using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class MagneticPlatform : StateBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private GameObject detectionPoint;
    [SerializeField] private Transform lookTransform;

    private bool HasTarget()
    {
        return Lockable != null;
    }

    public bool IsTargetInRange(Vector3 targetPosition)
    {
        float distance = Vector2.Distance(targetPosition, detectionPoint.transform.position);
        return distance <= radius;
    }

    public bool IsTargetViable()
    {
        if (HasTarget() && IsTargetInRange(Lockable.Transform.position)) return true;
        Release();
        return false;
    }

    public void Release(bool skipTargetTrigger = false)
    {
        SetStateTrigger("turnOff", true);
        if (!skipTargetTrigger)
        {
            Lockable?.StateTrigger("releaseLock", true);
        }
        Lockable = null;
    }

    public void SetStateTrigger(string triggerName, bool value)
    {
        StateMachine.SetTrigger(triggerName, value);
    }

    public float Radius => radius;
    public GameObject DetectionPoint => detectionPoint;
    public Transform LookTransform => lookTransform;
    public ILockable Lockable { get; set; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = HasTarget() ? Color.green : Color.red;
        Gizmos.DrawSphere(detectionPoint.transform.position, radius);
    }
}