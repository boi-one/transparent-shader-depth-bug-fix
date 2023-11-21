using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TValue> : Dictionary<string, TValue>, ISerializationCallbackReceiver
{
    [Serializable]
    private struct Dictstruct
    {
        public string name;
        public TValue value;
    };

    [SerializeField]
    private List<Dictstruct> _dict = new List<Dictstruct>();

    // Save the dictionary to the lists
    public void OnBeforeSerialize()
    {
        _dict.Clear();

        foreach(KeyValuePair<string, TValue> pair in this)
        {
            _dict.Add(new Dictstruct
            {
                name = pair.Key,
                value = pair.Value,
            });
        }
    }

    // Load dictionary from lists
    public void OnAfterDeserialize()
    {
        if ((typeof(TValue).Attributes & TypeAttributes.Serializable) == 0)
            throw new SerializationException($"\"{typeof(TValue)}\" is not serializable.\nAdd the \"[Serializable]\" Attribute to your object");
        
        
        this.Clear();

        var l = _dict.Count;
        for(int i = 0; i < l; i++)
        {
            var currentItem = _dict[i];

            if (ContainsKey(currentItem.name))
            {
                currentItem.name = $"item{i}";
            }

            this.Add(currentItem.name, currentItem.value);
        }
    }
}
