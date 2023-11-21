using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenUtilities
{
    public static Vector3 GetWorldDirection(Vector3 mousePos, Vector3 centerPoint)
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        return (mouseWorldPosition - (Vector2)centerPoint).normalized;
    }
}
