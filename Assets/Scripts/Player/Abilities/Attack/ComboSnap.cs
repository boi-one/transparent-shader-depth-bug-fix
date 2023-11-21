using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ComboSnap : MonoBehaviour
{
    private ForceBody _forceBody;
    private PlayerMovement _playerMovement;
    private HitSystem _hitSystem;

    [SerializeField] private Vector2 jumpForce;
    [SerializeField] private float snapRadius = 1.6f;
    [SerializeField] private float radiusOffsetY = 2;
    [SerializeField] private float enemySnapOffsetXMultiplier = 2;
    [SerializeField] private float enemySnapOffsetYPosition = 1;
    [SerializeField] private bool isDrawingGizmos;
    [SerializeField] private Force comboForce;

    private void Awake()
    {
        _forceBody = GetComponent<ForceBody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    
    public void AddComboForce()
    {
        if(_playerMovement.IsGrounded) return;

        var lookDirection = gameObject.GetDirection();
        var xDirection = jumpForce.x * lookDirection;
        
        comboForce.Direction = new Vector3(xDirection,jumpForce.y,0);

        _forceBody.ClearForces();
        _forceBody.Add(comboForce);
    }

    public void ChangeEnemyPosition()
    {
        var playerPosition = transform.position;
        var radiusPosition = new Vector2(playerPosition.x + gameObject.GetDirection(), playerPosition.y + radiusOffsetY);

        var closestEnemy = GetNearestEnemy(radiusPosition, playerPosition);
        
        if(closestEnemy == null) return;

        var enemyForceBody = closestEnemy.GetComponent<ForceBody>();    
        enemyForceBody.ClearForces();
        playerPosition = transform.position;
        
        var newEnemyPosition = playerPosition;
        newEnemyPosition.x += gameObject.GetDirection() * enemySnapOffsetXMultiplier;
        newEnemyPosition.y += enemySnapOffsetYPosition;

        var desiredDirection = new Vector3(-newEnemyPosition.x, 0);
        enemyForceBody.SetPosition(newEnemyPosition, desiredDirection);
    }

    public void ResetEnemyForce(GameObject enemy)
    {
        if(enemy.TryGetComponent(out ForceBody forceBody))
        {
            forceBody.ClearForces();
        }
    }

    public GameObject GetNearestEnemy(Vector2 radiusPosition, Vector2 playerPosition)
    {
        var enemies = PhysicsUtils.OverlapSphere<EnemyAI>(radiusPosition, snapRadius);

        if(enemies == null || enemies.Count == 0) return null;
        
        var smallestDistanceEnemy = Mathf.Infinity;
        var distanceBetween = Vector2.zero;
        var enemyDistance = 0f;
        var closestEnemy = gameObject;

        foreach (var enemy in enemies)
        {
            var enemyPosition = (Vector2)enemy.transform.position;
            distanceBetween = enemyPosition - playerPosition;
            enemyDistance = distanceBetween.sqrMagnitude;

            if (enemyDistance < smallestDistanceEnemy)
            {
                smallestDistanceEnemy = enemyDistance;
                closestEnemy = enemy.gameObject;
            }
        }
        
        return closestEnemy;
    }
    
    void OnDrawGizmos()
    {
        if(!isDrawingGizmos) return;
        
        var playerPosition = transform.position;
        var radiusPosition = new Vector2(playerPosition.x + GetComponent<MeleeAttack>().GetLookDirection(), playerPosition.y + radiusOffsetY);
       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(radiusPosition, snapRadius);
    }
}

