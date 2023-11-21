using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComponentCategorySettings))]
public class ComponentCollectionInspector : Editor
{
    [SerializeField] private ComponentCategorySettings componentCategorySettings;

    private void OnEnable()
    {
        componentCategorySettings.UpdateList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        componentCategorySettings.UpdateCollection();
    }
}