using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class ParameterContainer<T> : ScriptableObject
{
    [SerializeField] private string key;
    [SerializeField] private T value;

    public string Key
    {
        get => key;
        set
        {
            key = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public T Value
    {
        get => value;
        set
        {
            this.value = value;

#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }
}