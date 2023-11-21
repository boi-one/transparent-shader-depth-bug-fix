using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StateMachine;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(NodeConnectionData))]
public class NodeConnectionEditor : Editor
{
    private NodeConnectionData _data;
    private VisualElement _root;

    private TransitionData _selectedTransition;
    private VisualElement _selectedTransitionElement;

    private const string SelectedColor = "#9ebfc4";
    private const string DefaultColor = "#292929";

    private Button _addConditionButton;
    private Button _addTransitionButton;

    private const string UxmlSource = "Assets/Scripts/Framework/StateMachine/Editor/TransitionEditor.uxml";

    public override VisualElement CreateInspectorGUI()
    {
        _data = target as NodeConnectionData;
        if (_data == null) return null;

        _root = new VisualElement();
        
        _addConditionButton = _root.Q<Button>("AddCondition");
        _addTransitionButton = _root.Q<Button>("AddTransition");

        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlSource);
        VisualElement uxmlSource = visualTree.CloneTree();
        _root.Add(uxmlSource);

        _root.Q<Button>("AddTransition").clicked += () =>
        {
            _data.AddTransition();
            RedrawTransitions();
        };
        
        if (StateMachineController.Instance != null && StateMachineController.Instance.data.GetAllParameterKeys().IsEmpty()) 
            _root.Q<Button>("AddCondition").visible = false;
        
        _root.Q<Button>("AddCondition").clicked += () =>
        {
            _selectedTransition.AddCondition();
            RedrawConditions();
        };

        RedrawTransitions();
        RedrawConditions();

        return _root;
    }

    public void RedrawTransitions()
    {
        _root.Q<VisualElement>("TransitionsList").Clear();
        _data.transitions.ForEach(transitionData =>
        {
            Label label = new Label(_data.from + " -> " + _data.to);
            label.AddManipulator(new Clickable(() => SelectTransition(transitionData, label)));
            label.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("remove", action =>
                {
                    _data.RemoveTransition(transitionData);
                    RedrawTransitions();
                });
            }));
            _root.Q<VisualElement>("TransitionsList").Add(label);
            if (_selectedTransition == null) SelectTransition(transitionData, label);
        });
    }

    public void RedrawConditions()
    {
        _root.Q<VisualElement>("ConditionsList").Clear();
        if (_selectedTransition == null) return;
        _selectedTransition.conditions.ForEach(conditionData =>
        {
            if (StateMachineController.Instance is null) return;
            if (StateMachineController.Instance.data.GetAllParameterKeys().IsEmpty()) return;
            
            VisualElement element = new VisualElement();
            
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("remove", action =>
                {
                    _selectedTransition.RemoveCondition(conditionData);
                    RedrawConditions();
                });
            }));
            
            element.style.flexDirection = FlexDirection.Row;

            var parameterField = DrawParameterPopup(element, conditionData);

            DrawConditionData(element, parameterField.value, conditionData);

            _root.Q<VisualElement>("ConditionsList").Add(element);
        });
    }

    private void DrawConditionData(VisualElement container, string key, ConditionData data)
    {
        switch (GetParameterTypeFromKey(key))
        {
            case "bool":
                DrawToggleCondition(container, data);
                break;
            case "int":
                DrawIntCondition(container, data);
                break;
            case "float":
                DrawFloatCondition(container, data);
                break;
            case "vector2":
                DrawVector2Condition(container, data);
                break;
            case "vector3":
                DrawVector3Condition(container, data);
                break;
        }
    }

    private string GetParameterTypeFromKey(string key)
    {
        StateMachineData data = StateMachineController.Instance.data;
        if (data.bools.Exists(parameterContainer => parameterContainer.Key == key)) return "bool";
        if (data.triggers.Exists(parameterContainer => parameterContainer.Key == key)) return "bool";
        if (data.ints.Exists(parameterContainer => parameterContainer.Key == key)) return "int";
        if (data.floats.Exists(parameterContainer => parameterContainer.Key == key)) return "float";
        if (data.vector2S.Exists(parameterContainer => parameterContainer.Key == key)) return "vector2";
        if (data.vector3S.Exists(parameterContainer => parameterContainer.Key == key)) return "vector3";
        return null;
    }

    private void DrawToggleCondition(VisualElement container, ConditionData data)
    {
        container.Clear();
        DrawParameterPopup(container, data);
        DrawCheckTypePopup(container, data);
        if (data.Value == null) data.Value = true;
        Toggle field = new Toggle { value = (bool)data.Value };
        field.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
        container.Add(field);
    }

    private void DrawIntCondition(VisualElement container, ConditionData data)
    {
        container.Clear();
        DrawParameterPopup(container, data);
        DrawCheckTypePopup(container, data);
        if (data.Value == null) data.Value = 1;
        IntegerField field = new IntegerField { value = (int)data.Value };
        field.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
        container.Add(field);
    }

    private void DrawFloatCondition(VisualElement container, ConditionData data)
    {
        container.Clear();
        DrawParameterPopup(container, data);
        DrawCheckTypePopup(container, data);
        if (data.Value == null) data.Value = 1f;
        FloatField field = new FloatField() { value = (float)data.Value };
        field.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
        container.Add(field);
    }

    private void DrawVector2Condition(VisualElement container, ConditionData data)
    {
        container.Clear();
        DrawParameterPopup(container, data);
        DrawCheckTypePopup(container, data);
        if (data.Value == null) data.Value = Vector2.zero;
        Vector2Field field = new Vector2Field() { value = (Vector2)data.Value };
        field.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
        container.Add(field);
    }

    private void DrawVector3Condition(VisualElement container, ConditionData data)
    {
        container.Clear();
        DrawParameterPopup(container, data);
        DrawCheckTypePopup(container, data);
        if (data.Value == null) data.Value = Vector3.zero;
        Vector3Field field = new Vector3Field() { value = (Vector3)data.Value };
        field.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
        container.Add(field);
    }

    private PopupField<string> DrawParameterPopup(VisualElement container, ConditionData conditionData)
    {
        int index = string.IsNullOrEmpty(conditionData.Key)
            ? 0
            : StateMachineController.Instance.data.GetAllParameterKeys().IndexOf(conditionData.Key);

        PopupField<string> parametersPopup =
            new PopupField<string>("", StateMachineController.Instance.data.GetAllParameterKeys(), index);

        parametersPopup.RegisterValueChangedCallback(evt =>
        {
            conditionData.Value = StateMachineController.Instance.data.GetParameterValue(evt.newValue);
            conditionData.Key = evt.newValue;
            DrawConditionData(container, evt.newValue, conditionData);
        });

        container.Add(parametersPopup);
        return parametersPopup;
    }

    private PopupField<string> DrawCheckTypePopup(VisualElement container, ConditionData data)
    {
        List<string> validCheckTypes;

        if (string.IsNullOrEmpty(data.Key))
            data.Key = StateMachineController.Instance.data.GetAllParameterKeys().First(s => !string.IsNullOrEmpty(s));

        switch (GetParameterTypeFromKey(data.Key))
        {
            case "bool":
                validCheckTypes = new List<string>() { "equals", "notEquals" };
                break;
            case "int":
                validCheckTypes = new List<string>() { "equals", "notEquals", "greater", "smaller" };
                break;
            case "float":
                validCheckTypes = new List<string>() { "equals", "notEquals", "greater", "smaller" };
                break;
            case "vector2":
                validCheckTypes = new List<string>() { "equals", "notEquals" };
                break;
            case "vector3":
                validCheckTypes = new List<string>() { "equals", "notEquals" };
                break;
            default:
                validCheckTypes = new List<string>() { "ERROR" };
                break;
        }

        int index = string.IsNullOrEmpty(data.CheckType) ? 0 : validCheckTypes.IndexOf(data.CheckType);

        PopupField<string> checkTypePopup =
            new PopupField<string>("", validCheckTypes, index);

        checkTypePopup.RegisterValueChangedCallback(evt => data.CheckType = evt.newValue);
        container.Add(checkTypePopup);

        return checkTypePopup;
    }

    private void SelectTransition(TransitionData data, VisualElement element)
    {
        ColorUtility.TryParseHtmlString(DefaultColor, out var defaultColor);
        if (_selectedTransition != null) _selectedTransitionElement.style.backgroundColor = defaultColor;
        _selectedTransition = data;
        _selectedTransitionElement = element;
        ColorUtility.TryParseHtmlString(SelectedColor, out var selectedColor);
        _selectedTransitionElement.style.backgroundColor = selectedColor;
        RedrawConditions();
    }
}