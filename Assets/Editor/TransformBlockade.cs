using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class TransformBlockade : Editor
{
    private TransformBlockadeData _blockadeData;
    private Transform _targetTransform;
    private Vector3 _previousRotation;

    private void OnEnable()
    {
        _targetTransform = (Transform)target;
        _blockadeData = _targetTransform.GetComponent<TransformBlockadeData>();
        _previousRotation = _targetTransform.localEulerAngles;
    }

    public override void OnInspectorGUI()
    {
        if (_blockadeData == null)
        {
            DrawFields(_targetTransform);
            return;
        }

        DrawFields(_targetTransform, _blockadeData.BlockPosition, _blockadeData.BlockRotation, _blockadeData.BlockScale);

        DrawHelp(_blockadeData);
    }

    private void DrawFields(Transform targetTransform, bool hidePosition = false, bool hideRotation = false, bool hideScale = false)
    {
        EditorGUI.BeginChangeCheck();

        var newPosition = targetTransform.localPosition;
        var newRotation = _previousRotation;
        var oldRotation = newRotation;
        var newScale = targetTransform.localScale;

        if (!hidePosition) newPosition = EditorGUILayout.Vector3Field("Position", targetTransform.localPosition);
        if (!hideRotation) newRotation = EditorGUILayout.Vector3Field("Rotation", newRotation);
        if (!hideScale) newScale = EditorGUILayout.Vector3Field("Scale", targetTransform.localScale);

        if(oldRotation != newRotation) _previousRotation = newRotation;

        if (!EditorGUI.EndChangeCheck()) return;
        
        Undo.RecordObject(targetTransform, "Transform Changed");

        targetTransform.localPosition = newPosition;
        targetTransform.localEulerAngles = newRotation;
        targetTransform.localScale = newScale;
    }

    private void DrawHelp(TransformBlockadeData blockadeData)
    {
        if (!string.IsNullOrEmpty(blockadeData.Description)) EditorGUILayout.HelpBox(blockadeData.Description, MessageType.Info);

        if (blockadeData.ReferrerGameObject == null) return;

        if (GUILayout.Button("Select")) EditorGUIUtility.PingObject(blockadeData.ReferrerGameObject);
    }
}