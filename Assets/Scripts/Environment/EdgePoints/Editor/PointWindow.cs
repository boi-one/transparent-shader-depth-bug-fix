using Points.PointTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PointWindow : EditorWindow
{
    private Point _point;
    public Point Point
    {
        get => _point;
        set => _point = value;
    }
    
    public static PointWindow ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        PointWindow window = GetWindow<PointWindow>();
        window.Show();
        return window;
    }

    public void UpdateGui()
    {
        OnGUI();
    }

    void OnGUI()
    {
        if (_point == null)
        {
            EditorGUILayout.HelpBox("There is no point selected", MessageType.Warning);
            return;
        }

        GUILayout.Label("Point settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        Point.Name = EditorGUILayout.TextField("Name", Point.Name);
        Point.Radius = EditorGUILayout.FloatField("Radius", Point.Radius);
        Point.Diameter = EditorGUILayout.FloatField("Diameter", Point.Diameter);
        Point.LocalPosition = EditorGUILayout.Vector3Field("Position", Point.LocalPosition);
        Point.GameObject = EditorGUILayout.ObjectField("GameObject", _point.GameObject, typeof(GameObject), true) as GameObject;
        
        if (_point.GetType() == typeof(GrabbablePoint))
        {
            GrabbablePoint grabbablePoint = (GrabbablePoint)_point;
            if (grabbablePoint != null)
            {
                grabbablePoint.GrabType = (GrabType)EditorGUILayout.EnumPopup("grabtype: ", grabbablePoint.GrabType);
            }
        }
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Changed point data");
        }
    }
}