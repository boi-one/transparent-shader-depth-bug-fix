using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Skills.Grabbing;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class LedgeSnap : MonoBehaviour
{
    private bool _isInAir;

    [SerializeField] private float maxDistance = 0.1f;
    [SerializeField] private float radius = 3;
    private float _radiusGrabbable = 1.05f;

    private float _smallestDistance;
    private float _fallingThreshold = 0.2f;

    [SerializeField] private Vector2 ledgePointPosition;
    [SerializeField] private int enemyLayer = 16;
    
    private ForceBody _forceBody;
    private PlayerMovement _playerMovement;

    private void Start()
    {
        _forceBody = GetComponent<ForceBody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForLedgePoints();

        if(_playerMovement.CanJump) _playerMovement.CanJump = false;
        if (_smallestDistance >= _radiusGrabbable) return;

        _playerMovement.CanJump = true;
        if (_isInAir) JumpToLedge();
    }

    private void CheckForLedgePoints()
    {
        var grabbables = PhysicsUtils.OverlapSphereWithChildren<Grabbable>(transform.position, radius);
        
        _smallestDistance = Mathf.Infinity;

        for (int i = 0; i < grabbables.Count; i++)
        {
            var grabbable = grabbables[i];

            var gameobject = grabbable.gameObject;

            var isNotEnemy = gameobject.layer == enemyLayer;

            if (!isNotEnemy) //todo: Contain Enemy vervangen met TagManager
            {
                var points = gameobject.GetComponent<PointCreator>().pointHolder.points;

                var pointsLength = points.Count;
                for (int a = 0; a < pointsLength; a++)
                {
                    var currentGrabPointPosition = points[a].LocalPosition + gameobject.transform.position;
                    var distance = Vector2.Distance(currentGrabPointPosition, transform.position);

                    if (_smallestDistance > distance)
                    {
                        _smallestDistance = distance;
                        ledgePointPosition = currentGrabPointPosition;
                    }
                }
            }
        }        
    }

    private void JumpToLedge()
    {
        if(DesiredYVelocity < _fallingThreshold) return;
        if (DesiredXVelocity >= maxDistance && _smallestDistance < _radiusGrabbable && ledgePointPosition !=  Vector2.zero) SnapToLedge(ledgePointPosition);
    }
    

    private float DesiredXVelocity => Mathf.Abs(_forceBody.DesiredVelocity.x);

    private float DesiredYVelocity => Mathf.Abs(_forceBody.DesiredVelocity.y);

    private void SnapToLedge(Vector2 grabbable)
    {
        _forceBody.ClearForces();
        transform.SetPositionX(grabbable.x);
        transform.SetPositionY(grabbable.y);
        IsInAir = false;
    }
    

    public bool IsInAir
    {
        get => _isInAir;
        set => _isInAir = value;
    }
}