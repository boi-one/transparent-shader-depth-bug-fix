using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class ParticleData
{
    [Header("Particle Name")] 
    public string name;
    [Header("GameObject Variables")] 
    public GameObject prefab;
    public GameObject parent;

    [Header("Offset Variables")]
    public OffsetPositions offsetPositions = OffsetPositions.Custom;
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector3 rotation = new Vector3(0, 0, 0);

    [Header("Particle Variables")] 
    public bool isEnabled = true;
    public bool flipHorizontal;
    public bool isAutoOrienting;
    public float defaultLifeTime = 3.0f;
    public float particleFireRate = 0.5f;
    public float pauzeTime = 0.0f;
    public float delayedSpawnTime = 0.0f;
    public bool hasCoolDown = true;
    public bool isConstant;
    public bool canSpawn = true;
    [HideInInspector] public string lifeTime = "Lifetime";
    [HideInInspector] public VisualEffect newVFX;
    [HideInInspector] public ParticleSystem newParticleSystem; 
    [HideInInspector] public int id;
    [HideInInspector] public List<GameObject> instantiatedParticles = new List<GameObject>();
}

public enum OffsetPositions
{
    Top,
    Left,
    Right,
    Bottom,
    Custom
}
