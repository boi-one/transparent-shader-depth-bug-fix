using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PrefabDragAndDropTool : EditorWindow
{
    [MenuItem("NeoN/Prefab Drag and Drop Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PrefabDragAndDropTool));
    }

    private bool _isSnappingTo2DGrid = false;
    private GameObject _currentGameObject;
    private LayerMask _placementMask;
    private LayerMask _originLayer = default;

    private int _ignoreRaycastLayer = 2;
    
    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Drag and Drop Tool", EditorStyles.boldLabel);
        _isSnappingTo2DGrid = EditorGUILayout.Toggle("Snap to 2D grid", _isSnappingTo2DGrid);
        
        GUILayout.Label("Mask to place prefabs onto", EditorStyles.label);
        LayerMask tempMask = EditorGUILayout.MaskField( InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_placementMask), InternalEditorUtility.layers);
        _placementMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
    }
    private void DuringSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (!(e.type == EventType.DragUpdated || e.type == EventType.DragPerform))
            return;

        if (!(DragAndDrop.objectReferences[0] is GameObject prefab))
            return;

        if (_currentGameObject == null)
        {
            CreatePrefab(prefab);
        }
        
        UpdatePosition(sceneView);

        if (Event.current.type == EventType.DragPerform)
        {
            Drop(e);
        }
    }

    private void CreatePrefab(GameObject prefab)
    {
        _currentGameObject = Instantiate(prefab);
        _originLayer = _currentGameObject.layer;
        _currentGameObject.layer = _ignoreRaycastLayer;
    }
    private void UpdatePosition(SceneView sceneView)
    {
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y; // Flip Y-coordinate to match scene view

        Ray mouseRay = sceneView.camera.ScreenPointToRay(mousePosition);
        Vector3 newPosition = _currentGameObject.transform.position;

        if (_isSnappingTo2DGrid)
        {
            Vector3 mouseWorldPosition = sceneView.camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -sceneView.camera.transform.position.z));
            mouseWorldPosition.z = 0;
            newPosition = mouseWorldPosition;
        }
        else if (Physics.Raycast(mouseRay, out var hit, Mathf.Infinity, _placementMask))
        {
            newPosition = hit.point;
        }

        _currentGameObject.transform.position = newPosition;
    }

    private void Drop(Event e)
    {
        DragAndDrop.AcceptDrag();
        _currentGameObject.layer = _originLayer;
        Selection.activeGameObject = _currentGameObject;
        Undo.RegisterCreatedObjectUndo(_currentGameObject, "Create Prefab");
        _currentGameObject = null;
        e.Use();
    }
}