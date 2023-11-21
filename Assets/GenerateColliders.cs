using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateColliders : MonoBehaviour, IInitializable
{
    private Ferr2DT_PathTerrain _ferr2DtPathTerrain;

    public void Initialize()
    {
        TryGetComponent(out _ferr2DtPathTerrain);
        
        _ferr2DtPathTerrain.Build();
    }
}
