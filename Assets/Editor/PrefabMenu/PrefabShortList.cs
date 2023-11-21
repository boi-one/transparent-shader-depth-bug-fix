using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(PrefabCategory))]
public class PrefabShortList : EditorWindow
{
    
    private static readonly Vector2 WindowPosition = new Vector2(50f, 50f);
    private static readonly float WindowWidth = 200f;
    private static readonly float WindowHeight = 150f;
    
    private Vector2 _scrollPosition;

    private const string LabelString = "Prefab Categories:";

    private Dictionary<PrefabCategory, bool> _foldoutState = new Dictionary<PrefabCategory, bool>();

    [MenuItem("GameObject/HLO/PrefabShortlist", false, 0)]
    private static void Init()
    {
        EditorWindow window = GetWindow<PrefabShortList>();
        window.position = new Rect(WindowPosition.x, WindowPosition.y, WindowWidth, WindowHeight);
        window.Show(true);
    }

    private static void SpawnPrefab(GameObject prefab)
    {
        var camera = SceneView.lastActiveSceneView.camera;
        var worldPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));

        var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (newPrefab == null) return;
        
        var prefabPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);
        newPrefab.transform.position = prefabPosition;

        RefreshObject(newPrefab);
    }

    private static void RefreshObject(GameObject newPrefab)
    {
        IInitializable refreshable = newPrefab.GetComponent<IInitializable>();

        refreshable?.Initialize();
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        
        GUI.color = Color.green;
        GUI.contentColor = Color.white;
        GUILayout.Label(LabelString);
        GUI.color = Color.white;

        var categories = PrefabShortListData.Instance.PrefabCategories;
        RenderCategory(categories, Color.black);

        EditorGUILayout.EndScrollView();
    }

    private bool CheckListIsOpen(PrefabCategory targetList)
    {
        if (!_foldoutState.ContainsKey(targetList)) return false;
        return _foldoutState[targetList];
    }

    private void RenderCategory(List<PrefabCategory> targetCategories, Color backgroundColor)
    {
        var l = targetCategories.Count;
        for (var i = 0; i < l; i++)
        {
            var currentCategory = targetCategories[i];
            GUI.backgroundColor = backgroundColor;
            if (GUILayout.Button(currentCategory.category))
            {
                ToggleList(currentCategory);
            }

            if (CheckListIsOpen(currentCategory))
            {
                var newColor = backgroundColor;
                newColor.a -= 0.25f;
                if (currentCategory.subPrefabCategories.Count > 0)
                {
                    RenderCategory(currentCategory.subPrefabCategories, newColor);
                }

                RenderChildren(currentCategory);
            }
        }
    }

    private void RenderChildren(PrefabCategory targetCategory)
    {
        GUI.backgroundColor = Color.gray;
        GUI.color = Color.white;

        var l = targetCategory.prefabs.Length;
        for (var i = 0; i < l; i++)
        {
            var currentSelectionPrefab = targetCategory.prefabs[i];

            if (GUILayout.Button(currentSelectionPrefab.name))
            {
                SpawnPrefab(currentSelectionPrefab);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }

    private void ToggleList(PrefabCategory targetList)
    {
        if (!_foldoutState.ContainsKey(targetList))
        {
            _foldoutState[targetList] = true;
            return;
        }

        _foldoutState[targetList] = !_foldoutState[targetList];
    }
}