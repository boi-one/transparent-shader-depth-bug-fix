using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtils
{
    public static List<T> OverlapSphere<T>(Vector3 position, float radius)
    {
        return OverlapSphereLogic<T>(position, radius, Component<T>);
    }

    public static List<T> OverlapSphereWithChildren<T>(Vector3 position, float radius)
    {
        return OverlapSphereLogic<T>(position, radius, ComponentInChildren<T>);
    }

    public static List<T> OverlapSphereLogic<T>(Vector3 position, float radius, Func<Collider, T> getComponentMethod)
    {
        var colliders = Physics.OverlapSphere(position, radius);
        var components = new List<T>();
        var l = colliders.Length;
        for (int i = 0; i < l; i++)
        {
            var component = getComponentMethod(colliders[i]);
            if (component == null) continue;
            components.Add(component);
        }
        return components;
    }

    private static T ComponentInChildren<T>(Collider collider)
    {
        return collider.GetComponentInChildren<T>();
    }
    
    private static T Component<T>(Collider collider)
    {
        return collider.GetComponent<T>();
    }
}