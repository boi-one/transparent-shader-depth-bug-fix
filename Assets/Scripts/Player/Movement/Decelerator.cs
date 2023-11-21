using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;

public class Decelerator : MonoBehaviour
{
    [SerializeField] private Force decelerationForce;
    [SerializeField] private ForceBody targetForceBody;

    [SerializeField] private float forceMultiplier;
    [SerializeField] private Vector2 velocityThreshold;

    [SerializeField] private UnityEvent onDecelerationStart = new UnityEvent();

    public void Activate()
    {
        targetForceBody = GetComponent<ForceBody>();
        DecelerationCheck();
    }

    private void DecelerationCheck()
    {
        var velocity = targetForceBody.Velocity;
        if (targetForceBody.HasActiveForce(decelerationForce.Id)
            || !(-velocity.y > velocityThreshold.y) && !(Mathf.Abs(velocity.x) > velocityThreshold.x)) return;

        Decelerate(velocity);
    }

    private void Decelerate(Vector3 velocity)
    {
        onDecelerationStart?.Invoke();
        var forceDirection = (velocity.x * -velocity.y * forceMultiplier);
        decelerationForce.Direction = new Vector3(forceDirection, 0, 0);
        targetForceBody.Add(decelerationForce);
    }
}