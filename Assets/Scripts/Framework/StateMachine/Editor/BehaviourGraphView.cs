using System.Collections.Generic;
using System.Linq;
using StateMachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BehaviourGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourGraphView, UxmlTraits>
    {
    }

    public StateMachineData StateMachineData;
    public Vector2 localMousePosition;
    
    public BehaviourGraphView()
    {
        Insert(0, new GridBackground());
        InstantiateManipulators();

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Framework/StateMachine/Editor/StateMachineController.uss");
        styleSheets.Add(styleSheet);

        RegisterCallback<MouseDownEvent>(UpdateLocalMousePosition);
    }

    private void UpdateLocalMousePosition(MouseDownEvent evt)
    {
        localMousePosition = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
    }

    private void InstantiateManipulators()
    {
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }
    
    public void PopulateView(StateMachineData stateMachineData)
    {
        this.StateMachineData = stateMachineData;

        graphViewChanged -= OnGraphViewChange;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChange;
        
        StateMachineData.nodes.ForEach(CreateNodeView);

        StateMachineData.nodes.ForEach(CreateEdges);
    }

    private void CreateEdges(StateBehaviourNode node)
    {
        var children = new List<StateBehaviourNode>();
        node.Connections.ForEach(data => children.Add(StateMachineData.GetNodeByGuid(data.to)));
            
        children.ForEach(behaviourNode =>
        {
            var parentView = FindNodeView(node);
            var childView = FindNodeView(behaviourNode);
                
            Edge edge = parentView.output.ConnectTo(childView.input);
            AddElement(edge); ;
        });
    }
    
    NodeView FindNodeView(StateBehaviourNode node)
    {
        return GetNodeByGuid(node.Guid) as NodeView;
    }

    public override void AddToSelection(ISelectable selectable)
    {
        base.AddToSelection(selectable);

        Edge edge = selectable as Edge;
        if (edge != null)
        {
            ActiveEditorTracker.sharedTracker.isLocked = true;
            Selection.activeObject = GetTransitionDataFromEdge(edge);
            ActiveEditorTracker.sharedTracker.isLocked = false;
        }
    }

    private NodeConnectionData GetTransitionDataFromEdge(Edge edge)
    {
        NodeView parentView = edge.output.node as NodeView;
        NodeView childView = edge.input.node as NodeView;
        if (parentView == null || childView == null) return null;

        StateBehaviourNode node = parentView.Node;

        return node.Connections.FirstOrDefault(data => data.from == parentView.Node.Guid && data.to == childView.Node.Guid);
    }

    private GraphViewChange OnGraphViewChange(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(DeleteElement);
        }
        
        graphViewChange.edgesToCreate?.ForEach(CreateEdge);
        return graphViewChange;
    }

    private void CreateEdge(Edge edge)
    {
        NodeView parentView = edge.output.node as NodeView;
        NodeView childView = edge.input.node as NodeView;
        if (parentView == null || childView == null) return;
        StateMachineData.nodes[StateMachineData.nodes.IndexOf(parentView.Node)].AddConnection(parentView.Node, childView.Node);
    }

    private void DeleteElement(GraphElement graphElement)
    {
        if (graphElement is NodeView nodeView) StateMachineData.DeleteNode(nodeView.Node);
        if (!(graphElement is Edge edge)) return;
        if (!(edge.output.node is NodeView parentView) || !(edge.input.node is NodeView childView)) return;
                    
        StateMachineData.nodes[StateMachineData.nodes.IndexOf(parentView.Node)].RemoveConnection(parentView.Node, childView.Node);
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (StateMachineData == null)
        {
            Debug.LogError("No StateMachine selected, please select a StateMachine in the assets folder.");
            return;
        }
        evt.menu.AppendAction("State behaviour node", action =>
        {
            var node = StateMachineData.CreateNode();
            CreateNodeView(node, localMousePosition);
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var validPorts = ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        validPorts = validPorts.Where(port =>
        {
            NodeView parentView = startPort.node as NodeView;
            NodeView childView = port.node as NodeView;
            if (parentView == null || childView == null) return true;
            if (parentView.Node.Connections.Exists(data => data.to == childView.Node.Guid)) return false;
            return true;
        }).ToList();
        
        return validPorts;
    }

    void CreateNodeView(StateBehaviourNode node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }

    void CreateNodeView(StateBehaviourNode node, Vector2 position)
    {
        node.Position = position;
        NodeView nodeView = new NodeView(node);

        AddElement(nodeView);
    }
}
