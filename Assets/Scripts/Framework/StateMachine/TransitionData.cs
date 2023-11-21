using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEditor;
using UnityEngine;

public class TransitionData : ScriptableObject
{
    public List<ConditionData> conditions = new List<ConditionData>();
    public StateMachineData parentData;
    
#if (UNITY_EDITOR)
    public void AddCondition()
    {
        var condition = ScriptableObject.CreateInstance<ConditionData>();
        condition.name = nameof(ConditionData);

        condition.CheckType = "equals";
        conditions.Add(condition);
        
        AssetDatabase.AddObjectToAsset(condition, parentData);
        EditorUtility.SetDirty(this);
    }

    public void RemoveCondition(ConditionData conditionData)
    {
        conditions.Remove(conditionData);
        AssetDatabase.RemoveObjectFromAsset(conditionData);
        EditorUtility.SetDirty(this);
    }

    public void Clear()
    {
        for (var i = conditions.Count - 1; i >= 0; i--)
        {
            AssetDatabase.RemoveObjectFromAsset(conditions[i]);
            conditions.RemoveAt(i);
        }
        EditorUtility.SetDirty(this);
    }
#endif
}
