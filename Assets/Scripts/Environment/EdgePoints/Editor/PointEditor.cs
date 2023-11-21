using Points.PointTypes;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

#if UNITY_EDITOR
[CustomEditor(typeof(PointCreator))]
public class PointEditor : Editor
{
    PointCreator _creator;
    Vector3 _creatorPosition;
    GameObject _creatorGameObject;
    
    PointHolder _pointHolder;
    readonly SelectionInfo _selectionInfo = new SelectionInfo();
    private bool _needsRepaint;
    public UnityEvent onPointChange = new UnityEvent();
    private PointWindow _pointWindow;

    /// <summary>
    /// OnInspectorGui is a function we can overwrite to make a custom inspector
    /// by adding elements and changing the layout of these elements
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Reset"))
        {
            Undo.RecordObject(_creator, "Reset");
            _creator.Reset();
            _pointHolder = _creator.pointHolder;
        }

        if (GUILayout.Button("Generate edge points"))
        {
            Undo.RecordObject(_creator, "generated edge points");
            _creator.GenerateEdgePoints();
        }

        if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
    }

    /// <summary>
    /// OnSceneGui is the base function called everytime the scene is updated.
    /// </summary>
    void OnSceneGUI()
    {
        Event guiEvent = Event.current;
        if (guiEvent.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Input();
        Draw();

        if (_needsRepaint)
        {
            HandleUtility.Repaint();
            _needsRepaint = false;
        }
    }

    /// <summary>
    /// Checks all player input for removing, adding, dragging points etc..
    /// </summary>
    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePos = GetMousePositionFromWorld(guiEvent);

        //checking if we have a point selected if not we check if we are hovering over a point 
        if (!_selectionInfo.PointIsSelected) CheckMouseOverPoint(mousePos);

        if (guiEvent.button == 0)
        {
            _selectionInfo.IsRemoving = guiEvent.modifiers == EventModifiers.Control;
            if (guiEvent.type == EventType.MouseDown)
            {
                if (guiEvent.modifiers == EventModifiers.Control) RemovePoint();
                if (guiEvent.modifiers == EventModifiers.Shift) AddPoint(mousePos);
                if (guiEvent.modifiers == EventModifiers.None) HandleLeftMouseDown();
            }
            if (guiEvent.type == EventType.MouseDrag) HandleLeftMouseDrag(mousePos);
            if (guiEvent.type == EventType.MouseUp) HandleLeftMouseUp(mousePos);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1) HandleRightMouseDown();
    }

    /// <summary>
    /// When right mouse is down we check if its over an point if it is. we open editor menu for the selected point. 
    /// </summary>
    private void HandleRightMouseDown()
    {
        _pointWindow = PointWindow.ShowWindow();
        if (!_selectionInfo.MouseIsOverPoint || _selectionInfo.PointIndex == -1)
        {
            _pointWindow.Point = null;
            return;
        }
        _pointWindow.Point = _pointHolder[_selectionInfo.PointIndex];
    }
    
    /// <summary>
    /// removes the segment from _selectionInfo.pointIndex 
    /// </summary>
    void RemovePoint()
    {
        if (!_selectionInfo.MouseIsOverPoint || _selectionInfo.PointIndex == -1) return;
        Undo.RecordObject(_creator, "Remove Point");
        _pointHolder.RemovePoint(_selectionInfo.PointIndex);
        _needsRepaint = true;
        
        onPointChange.Invoke();
    }


    /// <summary>
    /// Adds a segment with anchor point on mousePosition 
    /// </summary>
    /// <param name="mousePos">mousePosition</param>
    void AddPoint(Vector3 mousePos)
    {
        Undo.RecordObject(_creator, "Add Point");
        Point point = _creator.createDraggablePoints ? new GrabbablePoint(mousePos, _creatorGameObject) : new Point(mousePos, _creatorGameObject);
        _creator.pointHolder.AddPoint(point);
        _selectionInfo.PointIsSelected = true;
        _selectionInfo.PointIndex = _pointHolder.PointLenght - 1;
        _needsRepaint = true;
        
        onPointChange.Invoke();
    }

    /// <summary>
    /// Checks if the mouse from x, y is over a point. 
    /// </summary>
    /// <param name="mousePosition">the current mousePosition</param>
    void CheckMouseOverPoint(Vector2 mousePosition)
    {
        _selectionInfo.PointIndex = -1;
        for (int i = 0; i < _pointHolder.PointLenght; i++)
        {
            if (!(Vector2.Distance(_pointHolder[i].Position, mousePosition) < _pointHolder[i].Diameter)) continue;
            _selectionInfo.MouseIsOverPoint = true;
            _selectionInfo.PointIndex = i;
            return;
        }
        onPointChange.Invoke();
    }

    /// <summary>
    /// When leftMouse button is pressed (gets called only when there are no other buttons pressed)
    /// </summary>
    private void HandleLeftMouseDown()
    {
        if (!_selectionInfo.MouseIsOverPoint || _selectionInfo.PointIndex == -1) return;
        _selectionInfo.PositionAtStartOfDrag = _pointHolder[_selectionInfo.PointIndex].Position;
        _selectionInfo.PointIsSelected = true;
        _needsRepaint = true;
        onPointChange.Invoke();
    }

    /// <summary>
    /// When leftMouse button is released. 
    /// </summary>
    /// <param name="mousePosition">mousePosition</param>
    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (_selectionInfo.PointIndex == -1) return;
        if (!_selectionInfo.PointIsSelected) return;

        _pointHolder[_selectionInfo.PointIndex].Position = (_selectionInfo.PositionAtStartOfDrag);
        Undo.RecordObject(_creator, "Move point");
        _pointHolder[_selectionInfo.PointIndex].Position = ((mousePosition));

        _pointHolder.MovePoint(_selectionInfo.PointIndex, (mousePosition));
        _selectionInfo.PointIsSelected = false;
        _selectionInfo.PointIndex = -1;
        _needsRepaint = true;
        onPointChange.Invoke();
    }

    /// <summary>
    /// When we move our mouse this function is called. it checks if we selected a point if yes. we move the point 
    /// </summary>
    /// <param name="mousePosition"></param>
    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (_selectionInfo.PointIndex == -1) return;
        if (!_selectionInfo.PointIsSelected) return;

        _pointHolder.MovePoint(_selectionInfo.PointIndex, mousePosition);
        _pointHolder[_selectionInfo.PointIndex].Position = (mousePosition);

        _needsRepaint = true;
        onPointChange.Invoke();
    }

    /// <summary>
    /// draw is the base functions that calls all other draw methods.
    /// </summary>
    void Draw()
    {
        Handles.BeginGUI();

        GUI.color = Ferr2D_Visual.HelpBoxColor;
        GUI.Box(new Rect(1,EditorGUIUtility.singleLineHeight + 4,205,EditorGUIUtility.singleLineHeight * 5), "", HelpBoxStyle);
        GUI.color = Color.white;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("\u2022 SHIFT + LeftClick to add point");
        EditorGUILayout.LabelField("\u2022 CTRL + LeftClick to remove point");
        EditorGUILayout.LabelField("\u2022 LeftClick + Drag to move point");
        EditorGUILayout.LabelField("\u2022 RightClick to edit point");
        
        Handles.EndGUI();
        for (int i = 0; i < _pointHolder.PointLenght; i++)
        {
            Handles.color = _creator.defaultColor;

            if (i == _selectionInfo.PointIndex) Handles.color = _creator.hoverColor;
            if (i == _selectionInfo.PointIndex && _selectionInfo.PointIsSelected) Handles.color = _creator.selectedColor;
            if (_selectionInfo.IsRemoving) Handles.color = _creator.removeColor;

            Handles.DrawSolidDisc(_pointHolder[i].Position, Vector3.forward, _pointHolder[i].Diameter);
            if(_creator.drawRadius) Handles.DrawWireDisc(_pointHolder[i].Position, Vector3.forward, _pointHolder[i].Radius);
        }
    }
    
    /// <summary>
    /// returns box styling 
    /// </summary>
    private GUIStyle HelpBoxStyle { get {
        var helpBoxStyle = new GUIStyle( GUI.skin.box )
        {
            normal = { background = EditorGUIUtility.whiteTexture }
        };
        return helpBoxStyle;
    } }

    /// <summary>
    /// gets the mousePosition given in x, y coordinates 
    /// </summary>
    /// <param name="guiEvent"></param>
    /// <returns>returns x,y positions</returns>
    private Vector3 GetMousePositionFromWorld(Event guiEvent)
    {
        Ray mouseray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0; //y
        float dstToDrawPlane = (drawPlaneHeight - mouseray.origin.z) / mouseray.direction.z; //z
        Vector3 mousePos = mouseray.GetPoint(dstToDrawPlane);
        return mousePos;
    }

    /// <summary>
    /// triggers when this editor script is enabled.
    /// </summary>
    void OnEnable()
    {
        _creator = (PointCreator)target;
        _creatorPosition = _creator.transform.position;
        _creatorGameObject = _creator.gameObject;
        
        if (_creator.pointHolder == null)
        {
            _creator.CreatePath();
        }

        _pointHolder = _creator.pointHolder;
        onPointChange.AddListener(_creator.FixCollider);
    }

    /// <summary>
    /// triggers when this editor script gets disabled 
    /// </summary>
    private void OnDisable()
    {
        if (_pointWindow == null) return;
        _pointWindow.Close();
    }
}

public class SelectionInfo
{
    public bool IsRemoving = false;
    public int PointIndex = -1;
    public bool MouseIsOverPoint;
    public bool PointIsSelected;
    public Vector3 PositionAtStartOfDrag;
}
#endif