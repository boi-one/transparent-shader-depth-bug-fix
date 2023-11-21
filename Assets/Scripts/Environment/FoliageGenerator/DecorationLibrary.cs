using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Vector3 = System.Numerics.Vector3;


public class DecorationLibrary : MonoBehaviour
{

    public DecorationDirectionSet[] database;
    
    [System.Serializable]
    public struct DecorationDirectionSet
    {
        public Ferr2DT_TerrainDirection direction;

        public List<DecorationSettings> decorationSettings;
    }
    
    public List<DecorationSettings> GetDecorationPrefabs(Ferr2DT_TerrainDirection dir)
    {
        var decorationPrefabs = new List<DecorationSettings>();
        
        var l = database.Length;
        for (int i = 0; i < l; i++)
        {
            Ferr2DT_TerrainDirection t = database[i].direction;

            var h = database[i].decorationSettings.Count;
            for (int j = 0; j < h; j++)
            {
                if (t != dir) break;
                decorationPrefabs.Add(database[i].decorationSettings[j]);
            }

        }

        return decorationPrefabs;
    }
    
}

[System.Serializable]
public struct DecorationSettings
{
    public DecorationSize decorationSize;
        
    [Header("Randomise size")] 
    public Vector2 minSize; 
    public Vector2 maxSize;
    [Header("Randomise Z rotation")]
    public Vector2 randomRotation;
    [Header("Randomise Y position")] 
    public float minY;
    public float maxY;
    [Header("Lock Rotation Axes")] 
    public  bool x;
    public  bool y;
    public  bool z;
        
    public GameObject prefab;
}
