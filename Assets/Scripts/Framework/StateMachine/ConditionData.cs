using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class ConditionData : ScriptableObject
{
    [SerializeField] private string key;
    [SerializeField] private string checkType;
    [SerializeField] private SerializableObject _value;

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

    public string CheckType
    {
        get => checkType;
        set
        {
            checkType = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public object Value
    {
        get => _value?.value;
        set
        {
            _value ??= new SerializableObject();
            this._value.value = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }
}