using System.IO;
using UnityEditor;
using UnityEngine;

public class StaticPopupWindow : EditorWindow
{
    public GameObject targetObject;
    private const string Filename = "Assets/Editor/CustomInspector/UnityLogo.png";

    private void OnEnable()
    {
        titleContent = new GUIContent("Change Static Flags");
    }

    private void OnGUI()
    {
        RenderIsStaticPopup();
    }

    public void RenderIsStaticPopup()
    {
        RenderUnityLogo();

        GUILayout.Space(10);
        RenderPopupText();

        GUILayout.Space(70);
        RenderPopupButtons();
    }

    private static void RenderUnityLogo()
    {
        var rawData = File.ReadAllBytes(Filename);
        var tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
        tex.LoadImage(rawData);
        var rect = new Rect(15, 15, 80, 80);
        GUI.DrawTexture(rect, tex);
    }

    private static void RenderPopupText()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(110);
        GUILayout.Label("Do you want to enable the static flags for all the child \nobjects as well?");
        GUILayout.EndHorizontal();
    }

    private void RenderPopupButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(75);
        var shouldChangeChildren = GUILayout.Button("Yes, change children");
        GUILayout.Space(10);
        var shouldOnlyChangeItself = GUILayout.Button("No, this object only");
        GUILayout.Space(10);
        var isCanceled = GUILayout.Button("Cancel");
        GUILayout.Space(10);
        GUILayout.EndHorizontal();

        if (isCanceled)
            Close();

        if (shouldChangeChildren)
            ChangeChilderen();

        if (shouldOnlyChangeItself)
            ChangeGameobject();
    }

    public void ChangeChilderen()
    {
        ChangeGameobject();
        foreach (var childObjects in targetObject.GetComponentsInChildren<Transform>()) childObjects.gameObject.isStatic = targetObject.isStatic;
    }

    public void ChangeGameobject()
    {
        targetObject.isStatic = !targetObject.isStatic;
        Close();
    }
}