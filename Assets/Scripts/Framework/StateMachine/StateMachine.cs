using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace StateMachine
{
    /// <summary>
    /// A abstract state machine, this will handle default state machine behaviour like switching states.
    /// </summary>
    public sealed class StateMachine : MonoBehaviour
    {
        [SerializeField] private StateMachineData data;

        private StateBehaviourNode _currentNode;
        private StateBehaviour _activeStateBehaviour;
        private bool _callOnExit = false;

        private void Awake()
        {
            data = data.Clone();
            
            data.nodes.ForEach(VerifyExistence);
            data.nodes.ForEach(DeActivateNode);
            
            _callOnExit = true;

            StateBehaviourNode startNode = data.nodes.First(node => node.IsEntryNode);
            _currentNode = startNode;

            ActivateNode(startNode);
        }

        public StateBehaviourNode CurrentNode => _currentNode;

        public bool SetBool(string key, bool value)
        {
            return SetParameter(data.bools, key, value);
        }
        
        public void SetBool(string keyValue)
        {
            string[] subs = keyValue.Split(':');
            SetParameter(data.bools, subs[0], subs[1] == "true");
        }
        
        public void SetTrigger(string keyValue)
        {
            string[] subs = keyValue.Split(':');
            SetParameter(data.triggers, subs[0], subs[1] == "true");
        }
        
        public void SetInt(string keyValue)
        {
            string[] subs = keyValue.Split(':');
            SetParameter(data.ints, subs[0], int.Parse(subs[1]));
        }
        
        public bool SetTrigger(string key, bool value)
        {
            return SetParameter(data.triggers, key, value);
        }

        public bool SetInt(string key, int value)
        {
            return SetParameter(data.ints, key, value);
        }

        public bool SetFloat(string key, float value)
        {
            return SetParameter(data.floats, key, value);
        }

        public bool SetVector2(string key, Vector2 value)
        {
            return SetParameter(data.vector2S, key, value);
        }

        public bool SetVector3(string key, Vector3 value)
        {
            return SetParameter(data.vector3S, key, value);
        }

        private bool SetParameter<T, TContainer>(List<TContainer> container, string key, T value) where TContainer : ParameterContainer<T>
        {
            TContainer parameterContainer = container.FirstOrDefault(parameterContainer => parameterContainer.Key == key);
            if (parameterContainer == null) return false;
            parameterContainer.Value = value;
            CheckTransitions();
            return true;
        }

        private void CheckTransitions()
        {
            var isTransitioning = false;
            _currentNode.Connections.ForEach(connectionData =>
            {
                connectionData.transitions.ForEach(transitionData =>
                {
                    if (isTransitioning || !IsValidTransition(transitionData)) return;
                    isTransitioning = true;
                    ResetTriggers(transitionData);
                    DeActivateNode(_currentNode);
                    _currentNode = data.GetNodeByGuid(connectionData.to);
                    ActivateNode(_currentNode);
                });
            });
        }

        private void ResetTriggers(TransitionData transitionData)
        {
            foreach (var conditionData in transitionData.conditions)
            {
                var targetTrigger = data.triggers.FirstOrDefault(parameterContainer => parameterContainer.Key == conditionData.Key);
                if (targetTrigger == null) continue;
                targetTrigger.Value = false;
            }
        }

        private bool IsValidTransition(TransitionData transitionData)
        {
            foreach (var conditionData in transitionData.conditions)
            {
                IComparable parameterValue = (IComparable)data.GetParameterValue(conditionData.Key);
                IComparable conditionValue = (IComparable)conditionData.Value;

                if (parameterValue == null || conditionData == null) return false;

                switch (conditionData.CheckType)
                {
                    case "equals":
                        if (parameterValue.CompareTo(conditionValue) != 0) return false;
                        break;
                    case "notEquals":
                        if (parameterValue.CompareTo(conditionValue) == 0) return false;
                        break;
                    case "greater":
                        if (parameterValue.CompareTo(conditionValue) != 1) return false;
                        break;
                    case "smaller":
                        if (parameterValue.CompareTo(conditionValue) != -1) return false;
                        break;
                }
            }

            return true;
        }

        private void ActivateNode(StateBehaviourNode node)
        {
            StateBehaviour component = GetComponent(node.StateBehaviourType.SystemType) as StateBehaviour;
            if (component == null) return;
            _activeStateBehaviour = component;
            component.enabled = true;
            component.OnEnter();
        }

        private void DeActivateNode(StateBehaviourNode node)
        {
            StateBehaviour component = GetComponent(node.StateBehaviourType.SystemType) as StateBehaviour;
            if (component == null) return;
            component.enabled = false;
            if(_callOnExit)component.OnExit();
        }
        private void VerifyExistence(StateBehaviourNode node)
        {
            StateBehaviour component = GetComponent(node.StateBehaviourType.SystemType) as StateBehaviour;
            if (component == null)
            {
                Debug.LogError(node.StateBehaviourType.SystemType + " in " + gameObject.name + " is not found");
            }
        }

        public StateBehaviour ActiveStateBehaviour => _activeStateBehaviour;
    }
}