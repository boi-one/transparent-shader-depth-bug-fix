using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Customizer : MonoBehaviour
{
    private Renderer _currentRenderer;

    [Serializable]
    public struct ConnectionSetting
    {
        public GameObject targetPrefab;
        public GameObject targetParent;
    }
    
    [Serializable]
    public struct ColorSetting
    {
        public GameObject target;
        public Color targetColor;
    }
    
    [Serializable]
    public struct CullingSetting
    {
        public SkinnedMeshRenderer targetMesh;
        public GameObject targetPrefab;
        public List<int> materialIds;
    }

    [Header("Settings")]
    [SerializeField] private List<ConnectionSetting> connectionSettings = new List<ConnectionSetting>();
    [SerializeField] private List<ColorSetting> colorSettings = new List<ColorSetting>();
    [SerializeField] private List<CullingSetting> cullingSettings = new List<CullingSetting>();

    [SerializeField] private Material alphaMaterial;

    void Start()
    {
        InitializeConnections();
        InitializeColors();
        InitializeCulling();
    }

    private void InitializeConnections()
    {
        for (int i = 0; i < connectionSettings.Count; i++)
        {
            var currentSetting = connectionSettings[i];
            ConnectToTarget(currentSetting.targetPrefab, currentSetting.targetParent);
        }
    }
    
    private void InitializeColors()
    {
        for (int i = 0; i < colorSettings.Count; i++)
        {
            var currentSetting = colorSettings[i];
            ChangeColor(currentSetting.target, currentSetting.targetColor);
        }
    }
    
    public void InitializeCulling()
    {
        for (int i = 0; i < cullingSettings.Count; i++)
        {
            var currentSetting = cullingSettings[i];
            var newSkinnedMeshRenderers = InstantiateSkinnedMeshRenderer(currentSetting.targetPrefab);

            CullMeshMaterials(currentSetting.materialIds, currentSetting.targetMesh);
            ConnectToBones(newSkinnedMeshRenderers,currentSetting.targetMesh);
        }
    }

    public void ConnectToTarget(GameObject targetPrefab, GameObject targetParent)
    {
        Dictionary<GameObject, GameObject> spawnedChildrenCache = new Dictionary<GameObject, GameObject>();
        
        if (spawnedChildrenCache.ContainsKey(targetParent))
        {
            Destroy(spawnedChildrenCache[targetParent]);
        }
        
        GameObject newTargetPrefab = Instantiate(targetPrefab);
        newTargetPrefab.transform.parent = targetParent.transform;
        newTargetPrefab.transform.ResetLocalRotation();
        newTargetPrefab.transform.ResetLocalPosition();
        spawnedChildrenCache[targetParent] = newTargetPrefab;
    }

    public void ChangeColor(GameObject target, Color targetColor)
    {
        _currentRenderer = target.GetComponentInChildren<Renderer>();
        ChangeColor(_currentRenderer, targetColor);
    }
  
    public void ChangeColor(Renderer targetRenderer, Color targetColor)
    {
        if (targetRenderer == null) return;
        
        Dictionary<Renderer, Material> rendererCache = new Dictionary<Renderer, Material>();
        
        Material currentMaterial;
        if (!rendererCache.ContainsKey(targetRenderer))
        {
            currentMaterial = _currentRenderer.material;
            rendererCache[targetRenderer] = currentMaterial;
        }
        currentMaterial = rendererCache[targetRenderer];
        SetColor(targetRenderer, currentMaterial, "_MainColor", targetColor);
    }

    private void SetColor(Renderer targetRenderer, Material targetMaterial, String propertyName, Color currentColor)
    {
        targetMaterial.SetColor(propertyName, currentColor);
        var materials = targetRenderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = targetMaterial;
        }
        targetRenderer.materials = materials;
    }

    private void CullMeshMaterials(List<int> targetIds, SkinnedMeshRenderer targetMeshRenderer)
    {
        if (targetIds.IsEmpty()) return;

        Material[] materials = targetMeshRenderer.materials;
        
        for (int i = 0; i < targetIds.Count; i++)
        {
            materials[targetIds[i]] = alphaMaterial;
        }

        targetMeshRenderer.materials = materials;
    }

    private List<SkinnedMeshRenderer> InstantiateSkinnedMeshRenderer(GameObject target)
    {
        var newSkinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        
        foreach (Transform child in target.transform)
        {
            if (!child.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer)) continue;
            
            var newSkinnedMeshRenderer = Instantiate(skinnedMeshRenderer);
            newSkinnedMeshRenderers.Add(newSkinnedMeshRenderer);
        }

        return newSkinnedMeshRenderers;
    }

    private void ConnectToBones(List<SkinnedMeshRenderer> targetList, SkinnedMeshRenderer targetMesh)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            var targetRootBone = targetMesh.rootBone;

            targetList[i].transform.parent = targetMesh.transform.parent;
            targetList[i].bones = targetMesh.bones;
            targetList[i].rootBone = targetRootBone;
        }
    }
    
    
}

