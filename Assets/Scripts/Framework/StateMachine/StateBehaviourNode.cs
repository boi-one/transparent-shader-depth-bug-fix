using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StateMachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class StateBehaviourNode : ScriptableObject
{
   [FormerlySerializedAs("_guid")] [SerializeField] private string guid;
   [FormerlySerializedAs("_position")] [SerializeField] private Vector2 position;
   [FormerlySerializedAs("_connections")] [SerializeField] private List<NodeConnectionData> connections = new List<NodeConnectionData>();
   [FormerlySerializedAs("_stateBehaviour")] [SerializeField] private Object stateBehaviour;
   [FormerlySerializedAs("_stateBehaviourType")] [SerializeField] private SerializableSystemType stateBehaviourType;
   [FormerlySerializedAs("_isEntryNode")] [SerializeField] private bool isEntryNode = false;
   [FormerlySerializedAs("_parentData")] [SerializeField] private StateMachineData parentData;
    
#if (UNITY_EDITOR)
    
    public void AddConnection(StateBehaviourNode parent, StateBehaviourNode child)
    {
        NodeConnectionData data = CreateInstance<NodeConnectionData>();
        data.from = parent.guid;
        data.to = child.guid;
        data.name = nameof(NodeConnectionData);
        data.parentData = parentData;

        connections.Add(data);

        AssetDatabase.AddObjectToAsset(data, parentData);
        AssetDatabase.SaveAssets();
    }

    public void RemoveConnection(StateBehaviourNode parent, StateBehaviourNode child)
    {
        NodeConnectionData nodeConnectionData = connections.FirstOrDefault(data => data.from == parent.guid && data.to == child.guid);
        if (nodeConnectionData == null) return;

        nodeConnectionData.Clear();
        connections.Remove(nodeConnectionData);

        AssetDatabase.RemoveObjectFromAsset(nodeConnectionData);
        AssetDatabase.SaveAssets();
    }
    
#endif
    
    #region Getters and setters

    
    public StateMachineData ParentData
    {
        get => parentData;
        set
        {
            parentData = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public SerializableSystemType StateBehaviourType
    {
        get => stateBehaviourType;
        set
        {
            stateBehaviourType = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public Object StateBehaviour
    {
        get => stateBehaviour;
        set
        {
            stateBehaviour = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public string Guid
    {
        get => guid;
        set
        {
            guid = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public bool IsEntryNode
    {
        get => isEntryNode;
        set
        {
            isEntryNode = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    public List<NodeConnectionData> Connections
    {
        get => connections;
        set
        {
            connections = value;
#if (UNITY_EDITOR)
            EditorUtility.SetDirty(this);
#endif
        }
    }

    #endregion
}