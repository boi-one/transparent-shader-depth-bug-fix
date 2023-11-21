using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShaderActivator : MonoBehaviour
{

    [Header("Shaders for Hit: ")] 
    
    private List<Material> _materials = new List<Material>();

    [SerializeField] private List<GameObject> childObjects;

    [SerializeField] private string materialName;

    private List<Material> _allMaterials = new List<Material>();

    private void Start()
    {
        GetAllMaterials();
    }

    private void GetAllMaterials()
    {
        foreach (var material in childObjects)
        {
            var materialsInObjects = material.GetComponent<SkinnedMeshRenderer>().materials;
            for (int i = 0; i < materialsInObjects.Length; i++) _allMaterials.Add(materialsInObjects[i]);
        }
        foreach (var material in _allMaterials)
        {
            if (material.name == materialName)
            {
                _materials.Add(material);
            }
        }   
    }
    
    public void Trigger()
    {
        int l = _materials.Count;

        for (int i = 0; i < l; i++)
        {
            var currentMaterial = _materials[i];
            currentMaterial.SetFloat("_RealTime" , Time.time);
        }   
    }
}
