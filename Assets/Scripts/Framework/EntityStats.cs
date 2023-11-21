using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private float strength = 1f;
    
    public float Strength
    {
        get => strength;
        set => strength = value;
    }
}
