using System;
using System.Collections.Generic;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class VelocityRotator : MonoBehaviour
{
    [SerializeField] private ForceBody forceBody;
    [SerializeField] private CharacterOrientation currentOrientation;
    [SerializeField] private bool isEnabled;
    [SerializeField] private float velocityTreshold = 0.0001f;
    [SerializeField] private bool autoReset;
    [SerializeField] private bool autoFlip;
    private bool _defaultEnabled;
    private int _currentFlipDirection;
    [SerializeField] private float maxTurnSpeed;
    private float _currentAngle;
    private float _difference;
    public CharacterOrientation CurrentOrientation
    {
        get => currentOrientation;
        set => currentOrientation = value;
    }

    private Dictionary<CharacterOrientation, int> _additionalRotation = new Dictionary<CharacterOrientation, int>()
    {
        {CharacterOrientation.HeadFirst, 270},
        {CharacterOrientation.FeetFirst, 90},
        {CharacterOrientation.Forward, 0},
        {CharacterOrientation.Backward, 180}
    };

    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            if (!isEnabled && autoReset) ResetOrientation();
        }
    }

    void Start()
    {
        _defaultEnabled = isEnabled;
        _currentAngle = Mathf.Atan2(transform.right.y,transform.right.x) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        if (IsEnabled) FollowVelocity();
        
    }
    private void ResetOrientation()
    {
        transform.rotation = Quaternion.identity;
    }

    private void FollowVelocity()
    {
        // todo: we zouden nog een deel van deze method in een utility class weg kunnen werken
        var velocity = CurrentDirection;

        if (velocity.sqrMagnitude < velocityTreshold) return;
        
        if (autoFlip) gameObject.Flip(velocity);
        
        var direction = transform.localScale.x;
        velocity *= direction;
        
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        var additionalAngle = _additionalRotation[CurrentOrientation];
        angle += additionalAngle * direction;
        _difference = angle - _currentAngle;
        var deltaAngle = Mathf.Min(Mathf.Abs(_difference), maxTurnSpeed  * Time.deltaTime);
        deltaAngle *= Math.Sign(_difference);
        var newAngle = _currentAngle + deltaAngle;
        transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
        _currentAngle = newAngle;
    }

    public virtual Vector3 CurrentDirection => forceBody.Velocity;

    public void Reset()
    {
        IsEnabled = _defaultEnabled;
    }

}