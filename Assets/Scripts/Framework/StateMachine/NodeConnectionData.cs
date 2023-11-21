using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEditor;
using UnityEngine;

public class NodeConnectionData : ScriptableObject
{
    public string from;
    public string to;
    public StateMachineData parentData;

    public List<TransitionData> transitions = new List<TransitionData>();

#if (UNITY_EDITOR)
    public void AddTransition()
    {
        var transition = ScriptableObject.CreateInstance<TransitionData>();
        transition.name = nameof(TransitionData);
        transition.parentData = parentData;

        transitions.Add(transition);

        AssetDatabase.AddObjectToAsset(transition, parentData);
        EditorUtility.SetDirty(this);
    }

    public void RemoveTransition(TransitionData transitionData)
    {
        transitions.Remove(transitionData);
        transitionData.Clear();
        AssetDatabase.RemoveObjectFromAsset(transitionData);
        EditorUtility.SetDirty(this);
    }

    public void Clear()
    {
        for (var i = transitions.Count - 1; i >= 0; i--)
        {
            transitions[i].Clear();
            AssetDatabase.RemoveObjectFromAsset(transitions[i]);
            transitions.RemoveAt(i);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}