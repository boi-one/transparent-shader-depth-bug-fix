using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void ResetTransformation(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public static void RotateToDirection(this Transform target, Vector3 direction, float additionAngle = 0f)
    {
        var angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        target.eulerAngles = new Vector3(0, 0, angleDegrees + additionAngle);
    }
    
    public static void ResetLocalRotation(this Transform trans)
    {
        trans.localRotation = Quaternion.identity;
    }
    
    public static void ResetLocalPosition(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
    }
}
