using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Skills.Grabbing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PointGeneratorWindow : EditorWindow
{
    [MenuItem("Tools/Points/RegeneratePoints from AutoGenerateEdgePoints")]
    public static void RegeneratePoints()
    {
        var autoGenerateEdgePointsScripts = FindObjectsOfType<AutoGenerateEdgePoints>();
        foreach (var autoGenerateScript in autoGenerateEdgePointsScripts)
        {
            autoGenerateScript.Generate();  
        }
    }
    
    [MenuItem("Tools/Points/Add Grabables To All Ferr2DPaths")]
    public static void AddGrabbablesToAllFerr2DPaths()
    {
        var autoGenerateEdgePointsScripts = FindObjectsOfType<Ferr2DT_PathTerrain>();
        if (autoGenerateEdgePointsScripts.IsEmpty()) return;

        List<GameObject> objs = new List<GameObject>();
        foreach (var autoGenerateEdgePointsScript in autoGenerateEdgePointsScripts)
        {
            objs.Add(autoGenerateEdgePointsScript.gameObject);
        }
        if (objs.IsEmpty()) return;
        
        Undo.RecordObject(objs[0] , "Base Undo");
        int undoID = Undo.GetCurrentGroup();

        foreach (var parent in objs)
        {
            if (parent.GetComponentInChildren<AutoGenerateEdgePoints>() || parent.GetComponentInChildren<Grabbable>()) continue;
            GameObject gameObject = new GameObject();
            gameObject.name = "Grabbable";
            gameObject.transform.parent = parent.transform;

            gameObject.GetOrAddComponent<Grabbable>();
            gameObject.GetOrAddComponent<AutoGenerateEdgePoints>();
            
            Undo.RegisterCreatedObjectUndo(gameObject, "New Undo");
            Undo.CollapseUndoOperations(undoID);
        }
    }
}