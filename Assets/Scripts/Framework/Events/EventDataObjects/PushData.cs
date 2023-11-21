using System;
using System.Collections.Generic;
using UnityEngine;

public class PushData : EventData
{
    private Vector3 _position;
    private float _pushCount;

    private List<GameObject> _pushedObjects;

    public PushData(Vector3 position, List<GameObject> pushedObjects)
    {
        _position = position;
        _pushedObjects = pushedObjects;

        _pushCount = Math.Max(0.5f, pushedObjects.Count);
        SetEventType(EventTypes.PUSH);
    }

    public float GetPushCount()
    {
        return _pushCount;
    }
    
    public List<GameObject> GetPushedObjects()
    {
        return _pushedObjects;
    }

    public Vector3 GetPosition()
    {
        return _position;
    }
}
