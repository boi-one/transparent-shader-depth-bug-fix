using System;
using UnityEngine;

[System.Serializable]
public class SpriteData
{
    public string name;
    public Vector3 offset;
    public bool flipHorizontal;
    public float cooldownDuration;
    public string spriteName = "";
    
    public GameObject prefab;
    [HideInInspector] public Animator animator;
}
