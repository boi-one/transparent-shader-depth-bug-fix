using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FOVVisualizer : MonoBehaviour
{
    private float _fieldOfView;
    [SerializeField] private int rayCount = 20;
    private FollowingTurret _followingLaser;
    private float _radius;
    void OnDrawGizmos()
    {
        _followingLaser = GetComponentInParent<FollowingTurret>();
        var totalDegreesFOV = 180;
        _fieldOfView = _followingLaser.DetectionAngle * totalDegreesFOV;
        Gizmos.color = new Color(1f, 0f, 0f, 2f);
        _radius = _followingLaser.DetectionRange;
        Gizmos.DrawWireSphere(transform.position, _radius);
        var angle = -_fieldOfView / 2;
        var angleIncrement = _fieldOfView / rayCount;
        
        for (var i = 0; i < rayCount; i++)
        {
            var direction = Quaternion.Euler(0f, 0f, angle) * transform.right;
            var hits = Physics.RaycastAll(transform.position, direction);
            Gizmos.color = Color.red;
            foreach (var hit in hits)
            {
                Gizmos.DrawLine(transform.position, hit.point);
            }
            angle += angleIncrement;
        }    
    }
}
