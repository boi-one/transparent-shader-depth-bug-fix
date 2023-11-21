using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[System.Serializable]
public class SerializableObject : ISerializationCallbackReceiver
{
    public Object value;
    public string serializableValue;
    public string type;
    
    public void OnBeforeSerialize()
    {
        if (value == null) return;
        serializableValue = value.ToString();

        if (value is int) type = "int";
        else if (value is float) type = "float";
        else if (value is bool) type = "bool";
        else if (value is Vector2) type = "vector2";
        else if (value is Vector3) type = "vector3";
    }

    public void OnAfterDeserialize()
    {
        switch (type)
        {
            case "int":
                value = int.Parse(serializableValue); 
                return;
            case "float":
                value = float.Parse(serializableValue);
                return;
            case "bool":
                value = bool.Parse(serializableValue);
                return;
        }
    }
}
