using UnityEditor;
using UnityEngine;

namespace Environment.FoliageGenerator
{
    [CustomEditor(typeof(global::DecorationGenerator)), CanEditMultipleObjects]
    public class DecorationGeneratorGUI : Editor
    {
        bool showOffset = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            global::DecorationGenerator decorationGenerator = (global::DecorationGenerator) target;
            
            EditorGUILayout.Space(10);
            showOffset = EditorGUILayout.Foldout(showOffset, "DECORATION OFFSET");
            if (showOffset)
            {
                decorationGenerator.OffsetLarge = EditorGUILayout.Vector2Field("Offset Large", decorationGenerator.OffsetLarge);
                decorationGenerator.OffsetMedium = EditorGUILayout.Vector2Field("Offset Medium", decorationGenerator.OffsetMedium);
                decorationGenerator.OffsetSmall = EditorGUILayout.Vector2Field("Offset Small", decorationGenerator.OffsetSmall);
            }
            
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Reset Decoration"))
            {
                decorationGenerator.ResetDecoration();
            } 
            if (GUILayout.Button("Delete Decoration"))
            {
                decorationGenerator.RemoveDecoration();
            }
        }
    }
}
