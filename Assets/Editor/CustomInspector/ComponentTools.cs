using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
 
public static class ComponentTools
{
    [MenuItem("CONTEXT/Component/HLO Collapse All")]
    private static void CollapseAll(MenuCommand command)
    {
        SetAllInspectorsExpanded((command.context as Component).gameObject, false);
    }
 
    [MenuItem("CONTEXT/Component/HLO Expand All")]
    private static void ExpandAll(MenuCommand command)
    {
        SetAllInspectorsExpanded((command.context as Component).gameObject, true);
    }
 
    public static void SetAllInspectorsExpanded(GameObject go, bool expanded)
    {
        Component[] components = go.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is Renderer renderer)
            {
                var mats = renderer.sharedMaterials;
                for (int i = 0; i < mats.Length; ++i)
                {
                    InternalEditorUtility.SetIsInspectorExpanded(mats[i], expanded);
                }
            }
            InternalEditorUtility.SetIsInspectorExpanded(component, expanded);
        }
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}