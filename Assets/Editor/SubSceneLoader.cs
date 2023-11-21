using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

#if UNITY_EDITOR

public class SubSceneLoader : MonoBehaviour
{
    private static string _subSceneSuffix = "StaticObjects";
    private static string _fileExtension = ".unity";

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnEditorSceneManagerSceneOpened;
    }

    private static void OnEditorSceneManagerSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        if (scene.name.Contains(_subSceneSuffix)) return;

        var filePath = scene.path.Replace(_fileExtension, "-" + _subSceneSuffix + _fileExtension);
        if (!File.Exists(filePath)) return;
        EditorSceneManager.OpenScene(filePath, OpenSceneMode.Additive);
    }
}
#endif