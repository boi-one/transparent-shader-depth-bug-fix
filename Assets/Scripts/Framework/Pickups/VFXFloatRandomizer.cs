using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class VFXFloatRandomizer : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private Vector2 randomNumberGeneratorBounds = new Vector2(0, 100);

    private const string ParameterName = "StartTime";
    
    void Start()
    {
        if (!vfx.HasFloat(ParameterName))
            throw new MissingMemberException($"The given reference `vfx` does not contain a float named {ParameterName}");
        
        vfx.SetFloat(ParameterName, Random.Range(randomNumberGeneratorBounds.x, randomNumberGeneratorBounds.y));
    }
}
