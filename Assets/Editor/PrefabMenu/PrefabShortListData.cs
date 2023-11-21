using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct PrefabCategory
{
    public string category;
    public GameObject[] prefabs;
    [FormerlySerializedAs("PrefabCategoriesList")] public List<PrefabCategory> subPrefabCategories;
}

[CreateAssetMenu(fileName = "PrefabCategorySettings", menuName = "ScriptableObjects/Prefab Category Settings", order = 1)]
public class PrefabShortListData : SingletonScriptableObject<PrefabShortListData>
{    
    [SerializeField] private List<PrefabCategory> prefabCategories;
    
    public List<PrefabCategory> PrefabCategories => prefabCategories;
}


