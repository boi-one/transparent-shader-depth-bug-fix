using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OneWayPlatform))]
[CanEditMultipleObjects]
public class OneWayPlatformGUI : Editor
{
    private SerializedProperty _showVisuals;
    private SerializedProperty _setOneWay;
    private SerializedProperty _visualsObject;

    private SerializedProperty _collisionLineColor;
    private SerializedProperty _collisionLineHeight;

    private SerializedProperty _angleColor;
    private SerializedProperty _angleSize;

    private string _helpBoxText;
    
    private void OnEnable()
    {
        _showVisuals = serializedObject.FindProperty("showVisualsGUI");
        _setOneWay = serializedObject.FindProperty("setOneWay");

        _visualsObject = serializedObject.FindProperty("visualsObject");

        _collisionLineColor = serializedObject.FindProperty("collisionLineColor");
        _collisionLineHeight = serializedObject.FindProperty("collisionLineHeight");

        _angleColor = serializedObject.FindProperty("angleColor");
        _angleSize = serializedObject.FindProperty("angleSize");

        _helpBoxText = null;
    }

    public override void OnInspectorGUI()
    {
        Color backgroundColor = GUI.backgroundColor;

        if (_helpBoxText != null) EditorGUILayout.HelpBox(_helpBoxText, MessageType.Warning);
        EditorGUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Using OneWay");

        GUI.backgroundColor = _setOneWay.boolValue ? Color.green : Color.gray;
        if (GUILayout.Button("ON")) EnableOneWay(true, "'OneWay'");
        
        GUI.backgroundColor = _setOneWay.boolValue ? Color.gray : Color.red;
        if (GUILayout.Button("OFF")) EnableOneWay(false, "'Untagged'");

        GUILayout.EndHorizontal();
        EditorGUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();

        GUI.backgroundColor = backgroundColor;
        base.OnInspectorGUI();

        _showVisuals.boolValue = EditorGUILayout.Foldout(_showVisuals.boolValue, "Visuals", EditorStyles.foldoutHeader);
        if (_showVisuals.boolValue)
        {
            _visualsObject.objectReferenceValue = (GameObject) EditorGUILayout.ObjectField("visualsObject", _visualsObject.objectReferenceValue, typeof(GameObject), true);
            EditorGUILayout.Space(20);

            _collisionLineColor.colorValue = EditorGUILayout.ColorField("collisionLineColor", _collisionLineColor.colorValue);
            _collisionLineHeight.floatValue = EditorGUILayout.FloatField("collisionLineHeight", _collisionLineHeight.floatValue);
            EditorGUILayout.Space(10);

            _angleColor.colorValue = EditorGUILayout.ColorField("angleColor", _angleColor.colorValue);
            _angleSize.floatValue = EditorGUILayout.FloatField("angleSize", _angleSize.floatValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void EnableOneWay(bool value, string tag)
    {
        _showVisuals.boolValue = value;
        _setOneWay.boolValue = value;
        _helpBoxText = "Tag is set to " + tag;
    }
}