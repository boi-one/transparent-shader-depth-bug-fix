using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderExtentions
{
    /// <summary>
    /// get all corner positions of a box collider2D
    /// </summary>
    /// <returns>Array of positions</returns>
    public static Vector3[] GetCornerPoints(this Collider2D collider)
    {
        var center = collider.bounds.center;
        var extend = collider.bounds.extents;
        
        var topLeft = center + new Vector3(-extend.x, extend.y, 0);
        var topRight = center + new Vector3(extend.x, extend.y, 0);;
        var bottomLeft = center + new Vector3(-extend.x, -extend.y, 0);;
        var bottomRight = center + new Vector3(extend.x, -extend.y, 0);;

        return new[] {topLeft, topRight, bottomLeft, bottomRight};
    }
}
