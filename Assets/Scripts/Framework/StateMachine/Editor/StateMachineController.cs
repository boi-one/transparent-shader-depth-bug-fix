using System;
using System.Collections.Generic;
using NUnit.Framework;
using StateMachine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

public class StateMachineController : EditorWindow
{
    public static StateMachineController instance;
    
    public BehaviourGraphView BehaviourGraphView;
    public InspectorView InspectorView;
    public StateMachineData data;
    public VisualElement root;

    public VisualElement selectedElement;
    public Object selectedDataObject;

    private const string WindowUxml = "Assets/Scripts/Framework/StateMachine/Editor/StateMachineController.uxml";
    private const string WindowUss = "Assets/Scripts/Framework/StateMachine/Editor/StateMachineController.uxml";
    
    [MenuItem("StateMachine/Controller")]
    public static void ShowExample()
    {
        StateMachineController window = GetWindow<StateMachineController>();
        instance = window;
        window.titleContent = new GUIContent("StateMachineController");
    }

    public void CreateGUI()
    {
        root = rootVisualElement;

        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(WindowUxml);
        VisualElement labelFromUxml = visualTree.CloneTree();

        MakeFullWindow(labelFromUxml);

        root.Add(labelFromUxml);

        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(WindowUss);
        root.styleSheets.Add(styleSheet);

        InspectorView = root.Q<InspectorView>();
        BehaviourGraphView = root.Q<BehaviourGraphView>();

        OnSelectionChange();
        RenderParameterPopup();
        OnSelectionChange();
    }

    private void MakeFullWindow(VisualElement element)
    {
        element.style.position = Position.Absolute;
        element.style.left = 0;
        element.style.right = 0;
        element.style.top = 0;
        element.style.bottom = 0;
    }
    
    private void RenderParameterPopup()
    {
        root.Q<ToolbarMenu>("AddParameterMenu").menu.MenuItems().Clear();
        if (data != null)
        {
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Float", action =>
            {
                CreateParameter(data.floats, 0.0f);
            });
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Bool", action =>
            {
                CreateParameter(data.bools, false);
            });
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Trigger", action =>
            {
                CreateParameter(data.triggers, false);
            });
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Int", action =>
            {
                CreateParameter(data.ints, 0);
            });
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Vector2", action =>
            {
                CreateParameter(data.vector2S, Vector2.zero);
            });
            root.Q<ToolbarMenu>("AddParameterMenu").menu.AppendAction("Add Vector3", action =>
            {
                CreateParameter(data.vector3S, Vector3.zero);
            });
            
            RenderParameters();
        }
    }

    private void CreateParameter<T, TContainer>(List<TContainer> container, T defaultValue) where TContainer : ParameterContainer<T>
    {
        data.AddParameter(container, GUID.Generate().ToString(), defaultValue);
        RenderParameters();
    }

    private void OnSelectionChange()
    {
        StateMachineData stateData = Selection.activeObject as StateMachineData;
        if (!stateData) return;

        data = stateData;
        BehaviourGraphView.PopulateView(stateData);
        RenderParameterPopup();
        RenderParameters();
    }

    private void RenderParameters()
    {
        root.Q<ScrollView>("Params").Clear();

        data.bools.ForEach(container => RenderParameter(new Toggle(), container));
        data.triggers.ForEach(container => RenderParameter(new Toggle(), container));
        data.ints.ForEach(container => RenderParameter(new IntegerField(), container));
        data.floats.ForEach(container => RenderParameter(new FloatField(), container));
        data.vector2S.ForEach(container => RenderParameter(new Vector2Field(), container));
        data.vector3S.ForEach(container => RenderParameter(new Vector3Field(), container));
    }

    private void RenderParameter<T>(BaseField<T> valueField, ParameterContainer<T> parameterContainer)
    {
        VisualElement container = new VisualElement();
        TextField keyField = new TextField();
        

        keyField.RegisterValueChangedCallback(evt => parameterContainer.Key = evt.newValue);
        valueField.RegisterValueChangedCallback(evt => parameterContainer.Value = evt.newValue);
        
        keyField.value = parameterContainer.Key;
        valueField.value = parameterContainer.Value;
        
        container.style.flexDirection = FlexDirection.Row;
        keyField.style.flexGrow = 1;
        valueField.style.width = 200;

        container.Add(keyField);
        container.Add(valueField);

        container.style.paddingBottom = 10f;

        root.Q<ScrollView>("Params").Add(container);
        
        container.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Remove", action =>
            {
                data.RemoveParameter(parameterContainer);
                RenderParameters();
            });
        }));
    }

    public static StateMachineController Instance
    {
        get
        {
            if (instance == null && HasOpenInstances<StateMachineController>())
            {
                instance = GetWindow<StateMachineController>();
            }
            return instance;
        }
    }
}