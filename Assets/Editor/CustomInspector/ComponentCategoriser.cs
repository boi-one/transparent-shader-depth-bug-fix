using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ComponentCategoriser : Editor
{
    private static readonly Dictionary<object, ComponentCategories> TargetComponents = new Dictionary<object, ComponentCategories>();
    public Dictionary<object, ComponentCategories> currentCategories = new Dictionary<object, ComponentCategories>();

    public ComponentCategorySettings componentCategorySettings;

    private static readonly List<string> _filteredComponentsNames = new List<string>();

    private const string _transformName = "UnityEngine.Transform";
    private readonly string _allCategory = ComponentCategories.All.ToString();

    private static void ClearTargetComponents()
    {
        TargetComponents.Clear();
    }

    public void RefreshComponents(GameObject targetGameObject, bool onEnable = false)
    {
        ClearTargetComponents();
        UpdateComponentCollection();
        UpdateTargetComponents(targetGameObject);
        if (onEnable) componentCategorySettings.UpdateCollection();
    }

    private static void AddNameToFilteredComponents(string name)
    {
        _filteredComponentsNames.Clear();

        _filteredComponentsNames.Add(name);
    }

    public List<string> GetTargetComponentNames(string targetCategory, string searchText = "")
    {
        AddNameToFilteredComponents(_transformName);

        foreach (var component in TargetComponents)
        {
            var componentName = component.Key.ToString();
            var componentKeyToLower = componentName.ToLower();
            var componentCategory = component.Value.ToString();

            if (_filteredComponentsNames.Contains(componentKeyToLower)) continue;

            if (componentKeyToLower.Contains(searchText.ToLower()) && targetCategory == _allCategory)
            {
                _filteredComponentsNames.Add(componentName);
                continue;
            }

            if (componentKeyToLower.Contains(searchText.ToLower()) && componentCategory == targetCategory) _filteredComponentsNames.Add(componentName);
        }

        return _filteredComponentsNames;
    }

    private bool IsAlreadyInCollection(string componentKey)
    {
        return currentCategories.ContainsKey(componentKey);
    }

    private void UpdateTargetComponents(GameObject targetGameObject)
    {
        if (targetGameObject == null) return;
        var targetComponentDatabase = targetGameObject.GetComponents<Component>();

        foreach (var component in targetComponentDatabase)
        {
            var componentKey = component.GetType().ToString();
            var alreadyInTargetDatabase = TargetComponents.ContainsKey(componentKey);

            if (alreadyInTargetDatabase) continue;

            if (IsAlreadyInCollection(componentKey))
                foreach (var databaseComponent in currentCategories)
                {
                    var databaseComponentKey = databaseComponent.Key.ToString();
                    var hasSameKey = databaseComponentKey == componentKey;

                    if (hasSameKey) TargetComponents.Add(componentKey, databaseComponent.Value);
                }
            else TargetComponents.Add(componentKey, ComponentCategories.Other);
        }
    }

    private void UpdateComponentCollection()
    {
        var componentDictionary = componentCategorySettings.componentCategorySettings;
        foreach (var element in componentDictionary)
        {
            foreach (var component in element.components)
            {
                var elementName = component.component.name;
                if (IsAlreadyInCollection(elementName)) continue;

                currentCategories.Add(elementName, element.category);
            }
        }
    }

    public void SaveAllComponents(GameObject targetObject)
    {
        var allTargetScripts = targetObject.GetComponents<MonoBehaviour>();

        if (componentCategorySettings.componentCategorySettings.IsEmpty()) return;
        var componentDictionary = componentCategorySettings.componentCategorySettings[ComponentCategorySettings.GetCategoryIndex(ComponentCategories.Other)];

        foreach (var targetScript in allTargetScripts)
        {
            var targetMonoScript = MonoScript.FromMonoBehaviour(targetScript);
            if (ComponentCategorySettings.ContainsScript(targetMonoScript)) continue;

            ComponentCategorySettings.AddScript(targetMonoScript);

            var newComponentSetting = new ComponentSettings
            {
                name = targetMonoScript.name,
                category = ComponentCategories.Other,
                component = targetMonoScript
            };
            componentDictionary.components.Add(newComponentSetting);
        }
    }
}