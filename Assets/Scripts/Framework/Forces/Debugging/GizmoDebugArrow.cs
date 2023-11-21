using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

public class GizmoDebugArrow : DebugArrow
{
    [SerializeField] private float arrowHeadLength = 0.5f;
    public override Color Color { get; set; }
    public override Vector2 Direction => Force.CurrentForce == Vector3.zero ? Force.Direction : Force.CurrentForce;
    public override ForceDebugInfo DebugInfo { get; set; }

    private void DrawGizmosArrow()
    {
        var position = BodyDebugInfo.ForceBody.transform.position + Offset;
        var direction = BodyDebugInfo.ForceBody.transform.TransformDirection(Direction) * ScaleModifier;
        Gizmos.color = Color;
        Gizmos.DrawRay(position, direction);

        var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
        var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;
        
        Gizmos.DrawRay(position + direction, right * arrowHeadLength);
        Gizmos.DrawRay(position - direction, left * arrowHeadLength);
    }

    private void OnDrawGizmos()
    {
        DrawGizmosArrow();
    }
}
