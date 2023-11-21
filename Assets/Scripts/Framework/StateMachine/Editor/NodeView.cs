using StateMachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public StateBehaviourNode Node;
    public Port input;
    public Port output;

    private const string EntryNodeColor = "#D51A08";

    public NodeView(StateBehaviourNode node)
    {
        Node = node;
        this.title = node.name;
        this.viewDataKey = node.Guid;

        SetSpawnPosition();
        GenerateObjectField();
        CheckEntryNodeColor();
        CreatePort();
    }

    private void SetSpawnPosition()
    {
        style.left = Node.Position.x;
        style.top = Node.Position.y;
    }

    private void GenerateObjectField()
    {
        var objectField = new ObjectField() {objectType = typeof(MonoScript), value = Node.StateBehaviour};
        Node.StateBehaviour = objectField.value;
        
        if (Node.StateBehaviour != null)
            Node.StateBehaviourType = new SerializableSystemType(((MonoScript)Node.StateBehaviour)?.GetClass());
        
        objectField.RegisterValueChangedCallback(evt =>
        {
            Node.StateBehaviour = evt.newValue;
            if (Node.StateBehaviour != null)
                Node.StateBehaviourType = new SerializableSystemType(((MonoScript)Node.StateBehaviour)?.GetClass());
        });
        this.Add(objectField);
    }

    private void CheckEntryNodeColor()
    {
        if (Node.IsEntryNode)
        {
            ColorUtility.TryParseHtmlString(EntryNodeColor, out var selectedColor);
            this.style.backgroundColor = selectedColor;
        }
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (StateMachineController.Instance.data == null)
        {
            Debug.LogError("No StateMachine selected, please select a StateMachine in the assets folder.");
            return;
        }
        evt.menu.AppendAction("Set entry node", action =>
        {
            StateMachineController.Instance.data.nodes.ForEach(node => node.IsEntryNode = false);
            Node.IsEntryNode = true;
            if (StateMachineController.Instance.BehaviourGraphView != null && StateMachineController.Instance.data != null)
                StateMachineController.Instance.BehaviourGraphView.PopulateView(StateMachineController.Instance.data);
        });
    }

    private void CreatePort()
    {
        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        input.portName = "";
        inputContainer.Add(input);
        
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        output.portName = "";
        outputContainer.Add(output);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.Position = new Vector2(newPos.xMin, newPos.yMin);
    }
}
