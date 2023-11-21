using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderBoundsCalculator : MonoBehaviour
{
    [SerializeField] private string maxPropertyName;
    [SerializeField] private string minPropertyName;
    private List<Material> _materials = new List<Material>();
    private Bounds _bounds;
    private Renderer[] _renderers;
    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        GetMaterials();
        CalculateBounds();
        SetBounds();
    }

    private void GetMaterials()
    {
        _materials.Add(_renderers[0].material);
        for (int i = 1; i < _renderers.Length; i++)
        {
            _materials.Add(_renderers[i].material);
        }
    }
    
    private void CalculateBounds()
    {
        _bounds = _renderers[0].bounds;
        for (int i = 1; i < _renderers.Length; i++)
        {
            _bounds.Encapsulate(_renderers[i].bounds);
        }
    }
    private void SetBounds()
    {
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetVector(minPropertyName,_bounds.min);
            _materials[i].SetVector(maxPropertyName,_bounds.max);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_bounds.center, _bounds.size);
    }
}