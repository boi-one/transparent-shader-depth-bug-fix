using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GUILayout;
using AssetDatabase = UnityEditor.AssetDatabase;

public class SceneCreatorTool : EditorWindow
{
    private static readonly Vector2 WindowPosition = new Vector2(200f, 200f);
    private static readonly float WindowWidth = 300f;
    private static readonly float WindowHeight = 100f;
    private static readonly RectOffset WindowPadding =  new RectOffset(10, 10, 10, 10);
    private static Rect _windowRect;

    private string _templateScenePath = "Assets/Scenes/templates/TemplateScene-RedHill.unity";
    private string _staticTemplateScenePath = "Assets/Scenes/templates/TemplateScene-RedHill-StaticObjects.unity";
    private string _rootFolder = "Assets/Scenes";
    private string _staticSceneName = "StaticObjects";
    
    private string _name;
    private int _folderIndex;
    private Vector2 _scrollPosition;

    [MenuItem("NeoN/Scene Creator")]
    private static void Init()
    {
        EditorWindow window = GetWindow<SceneCreatorTool>();
        window.position = new Rect(WindowPosition.x, WindowPosition.y, WindowWidth, WindowHeight);
        _windowRect = new Rect(WindowPadding.right, WindowPadding.top, window.position.width - (WindowPadding.right + WindowPadding.left), window.position.height - (WindowPadding.top + WindowPadding.bottom));
        window.Show(true);
    }
    
    private void OnGUI()
    { 
        GUI.BeginGroup(_windowRect);
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        _name = EditorGUILayout.TextField("Scene name", _name, MinWidth(250));

        var sceneSubFolders = GetSceneSubFolders();

        _folderIndex = EditorGUILayout.Popup("Select Folder",_folderIndex, sceneSubFolders, MinWidth(250));
        
        if (Button("Create scene", MinWidth(250)))
        {
            StartCreatingNewScene(_rootFolder + "/" + sceneSubFolders[_folderIndex]);
        }

        EditorGUILayout.EndScrollView();
        GUI.EndGroup();
    }
    
    private void StartCreatingNewScene(string assetPath)
    {
        CopyScene(assetPath);
        
        EditorSceneManager.sceneOpened += OnLoaded;
        EditorSceneManager.OpenScene($"{assetPath}/{_name}.unity");

        void OnLoaded(Scene createdScene, OpenSceneMode ignoreMe)
        {
            if (createdScene.name.EndsWith(_staticSceneName)) return;
            
            EditorSceneManager.sceneOpened -= OnLoaded;

            var terrains = GetTerrain();
            foreach(var terrain in terrains)
                CopyTerrainData(terrain);
        }
    }

    private void CopyScene(string path)
    {
        AssetDatabase.CopyAsset(_templateScenePath, $"{path}/{_name}.unity");
        AssetDatabase.CopyAsset(_staticTemplateScenePath, $"{path}/{_name}-{_staticSceneName}.unity");
    }

    private void CopyTerrainData(Transform terrain)
    {
        var colliderComponent = terrain.GetComponent<TerrainCollider>();
        var terrainComponent = terrain.GetComponent<Terrain>();
        
        var terrainDataPath = AssetDatabase.GetAssetPath(colliderComponent.terrainData); 
        
        AssetDatabase.CopyAsset(terrainDataPath, terrainDataPath + "-" +_name + "-" + "TerrainData.asset");

        var newTerrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(terrainDataPath + "-" +_name + "-" + "TerrainData.asset");
        colliderComponent.terrainData = newTerrainData;
        terrainComponent.terrainData = newTerrainData;
    }

    private List<Transform> GetTerrain()
    {
        var listOfTerrains = new List<Transform>();
        var levelVisuals = GameObject.Find("SaveableObjects").transform.Find("level_visuals");

        var terrains = levelVisuals.Find("Terrain").GetComponentsInChildren<Terrain>();

        foreach (var terrain in terrains)
        {
            listOfTerrains.Add(terrain.transform);
        }

        return listOfTerrains;
    }

    private string[] GetSceneSubFolders()
    {
        var folders = GetFolders(_rootFolder).ToArray();
        
        for (var i = 0; i < folders.Length; i++)
        {
            folders[i] = folders[i].Remove(0, _rootFolder.Length);
            folders[i] = folders[i].TrimStart('/');
        }
        return folders.Where(c => c != "").ToArray();
    }


    private List<string> GetFolders(string root)
    {
        var result = new List<string> { root };
        result.AddRange(GetSubFolders(root));
        return result;
    }
    
    private List<string> GetSubFolders(string folder)
    {
        var subFolders = AssetDatabase.GetSubFolders(folder).ToList();
        for (var index = subFolders.Count - 1; index >= 0; index--)
        {
            var subFolder = subFolders[index];
            subFolders.AddRange(GetSubFolders(subFolder));
        }

        return subFolders;
    }
}
