using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace StateMachine
{
    [CreateAssetMenu(fileName = "StateMachine", menuName = "ScriptableObjects/StateMachine", order = 1)]
    public class StateMachineData : ScriptableObject
    {
        #region Parameters

        public List<BoolParameterContainer> bools = new List<BoolParameterContainer>();
        public List<BoolParameterContainer> triggers = new List<BoolParameterContainer>();
        public List<IntParameterContainer> ints = new List<IntParameterContainer>();
        public List<FloatParameterContainer> floats = new List<FloatParameterContainer>();
        public List<Vector2ParameterContainer> vector2S = new List<Vector2ParameterContainer>();
        public List<Vector3ParameterContainer> vector3S = new List<Vector3ParameterContainer>();

        #endregion

        public List<StateBehaviourNode> nodes = new List<StateBehaviourNode>();

#if (UNITY_EDITOR)

        public StateBehaviourNode CreateNode()
        {
            StateBehaviourNode node = ScriptableObject.CreateInstance<StateBehaviourNode>();
            node.name = nameof(StateBehaviourNode);
            node.Guid = GUID.Generate().ToString();
            nodes.Add(node);
            node.ParentData = this;

            if (!nodes.Exists(behaviourNode => behaviourNode.IsEntryNode)) node.IsEntryNode = true;

            AssetDatabase.AddObjectToAsset(node, this);
            EditorUtility.SetDirty(this);
            return node;
        }

        public void DeleteNode(StateBehaviourNode node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            EditorUtility.SetDirty(this);
        }

        public void AddParameter<T, TContainer>(List<TContainer> container, string key, T value)
            where TContainer : ScriptableObject
        {
            TContainer instance = ScriptableObject.CreateInstance<TContainer>();
            ParameterContainer<T> parameterContainer = instance as ParameterContainer<T>;

            parameterContainer.name = nameof(TContainer);
            parameterContainer.Key = key;
            parameterContainer.Value = value;

            container.Add(instance);

            AssetDatabase.AddObjectToAsset(instance, this);
            EditorUtility.SetDirty(this);
        }

        public void RemoveParameter<T, TContainer>(List<TContainer> container, TContainer parameterContainer)
            where TContainer : ScriptableObject
        {
            container.Remove(parameterContainer);
            ParameterContainer<T> scriptableContainer = parameterContainer as ParameterContainer<T>;
            AssetDatabase.RemoveObjectFromAsset(scriptableContainer);
            EditorUtility.SetDirty(this);
        }

        public void RemoveParameter<T>(ParameterContainer<T> container)
        {
            bools.Remove(container as BoolParameterContainer);
            triggers.Remove(container as BoolParameterContainer);
            ints.Remove(container as IntParameterContainer);
            floats.Remove(container as FloatParameterContainer);
            vector2S.Remove(container as Vector2ParameterContainer);
            vector3S.Remove(container as Vector3ParameterContainer);
            AssetDatabase.RemoveObjectFromAsset(container);
            EditorUtility.SetDirty(this);
        }
        
#endif
        
        public StateBehaviourNode GetNodeByGuid(string guid)
        {
            return nodes.FirstOrDefault(node => node.Guid == guid);
        }
        
        public Object GetParameterValue(string key)
        {
            if (bools.Exists(data => data.Key == key)) return bools.First(data => data.Key == key).Value;
            if (triggers.Exists(data => data.Key == key)) return triggers.First(data => data.Key == key).Value;
            if (ints.Exists(data => data.Key == key)) return ints.First(data => data.Key == key).Value;
            if (floats.Exists(data => data.Key == key)) return floats.First(data => data.Key == key).Value;
            if (vector2S.Exists(data => data.Key == key)) return vector2S.First(data => data.Key == key).Value;
            if (vector3S.Exists(data => data.Key == key)) return vector3S.First(data => data.Key == key).Value;
            return null;
        }

        public List<string> GetAllParameterKeys()
        {
            List<string> result = new List<string>();
            bools.ForEach(data => result.Add(data.Key));
            triggers.ForEach(data => result.Add(data.Key));
            ints.ForEach(data => result.Add(data.Key));
            floats.ForEach(data => result.Add(data.Key));
            vector2S.ForEach(data => result.Add(data.Key));
            vector3S.ForEach(data => result.Add(data.Key));
            return result;
        }
        
        public List<TContainer> CloneList<TContainer>(List<TContainer> container)
            where TContainer : ScriptableObject
        {
            var newList = new List<TContainer>();
            for (int i = 0; i < container.Count; i++)
            {
                newList.Add(Instantiate(container[i]));
            }
            return newList;
        }
        
        public StateMachineData Clone()
        {
            var newData = Instantiate(this);
            
            newData.bools = CloneList(bools);
            newData.triggers = CloneList(triggers);
            newData.ints = CloneList(ints);
            newData.floats = CloneList(floats);
            newData.vector2S = CloneList(vector2S);
            newData.vector3S = CloneList(vector3S);
            return newData;
        }

    }
}