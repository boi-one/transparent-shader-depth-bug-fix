using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct ComponentCategory
{
    [HideInInspector] public string name;
    public ComponentCategories category; // value
    public List<ComponentSettings> components; // key
}

[System.Serializable]
public struct ComponentSettings
{
    [HideInInspector] public string name;
    public ComponentCategories category; // value
    public MonoScript component; // key
}

[CreateAssetMenu(fileName = "Component Category Settings", menuName = "Component Category Settings")]
public class ComponentCategorySettings : ScriptableObject
{
    public List<ComponentCategory> componentCategorySettings = new List<ComponentCategory>();  
    private static List<MonoScript> _allComponents = new List<MonoScript>();

    private static List<ComponentCategories> _allCategories = new List<ComponentCategories>();

    public static bool ContainsScript(MonoScript targetScript)
    {
        return _allComponents.Contains(targetScript);
    }

    private static bool ContainsCategory(ComponentCategories category)
    {
        return _allCategories.Contains(category);
    }

    private static ComponentSettings CreateComponent(ComponentSettings settings)
    {
        var newComponent = new ComponentSettings()
        {
            name = settings.component.name,
            category = settings.category,
            component = settings.component
        };

        return newComponent;
    }

    public static void AddScript(MonoScript targetScript)
    {
        _allComponents.Add(targetScript);
    }

    public static void RemoveScript(MonoScript targetScript)
    {
        _allComponents.Remove(targetScript);
    }

    private void AddCategory(ComponentCategories category, params ComponentSettings[] components)
    {
        if (ContainsCategory(category)) return;

        var newCategory = new ComponentCategory
        {
            name = category.ToString(),
            category = category,
            components = components.ToList()
        };
        componentCategorySettings.Add(newCategory);
                    
        _allCategories.Add(newCategory.category);
    }

    private void MoveComponents(List<ComponentSettings> components, ComponentCategories category)
    {
        if (!ContainsCategory(category))
        {
            AddCategory(category, components.ToArray());
            return;
        }
        
        var newCategoryIndex = 0;
        foreach (var componentSettings in components)
        {
            var newComponentSettings = componentSettings;
            newComponentSettings.category = category;
            newCategoryIndex = GetCategoryIndex(category);
            componentCategorySettings[newCategoryIndex].components.Add(CreateComponent(newComponentSettings));
        }
    }
    
    private void MoveComponents(ComponentSettings component, ComponentCategories category)
    {
        if (!ContainsCategory(category))
        {
            AddCategory(category, component);
            return;
        }
        
        var categoryIndex = GetCategoryIndex(category);
        componentCategorySettings[categoryIndex].components.Remove(component);
        
        component.category = category;
        
        categoryIndex = GetCategoryIndex(category);
        componentCategorySettings[categoryIndex].components.Add(component);
    }

    public void UpdateList()
    {
        _allComponents = GetAllCurrentComponents();
        _allCategories = GetCurrentCategories();
    }

    private static void ClearList()
    {
        _allComponents.Clear();
        _allCategories.Clear();
    }

    public static int GetCategoryIndex(ComponentCategories category)
    {
        return _allCategories.IndexOf(category);
    }

    private List<MonoScript> GetAllCurrentComponents()
    {
        var allCurrentComponents = new List<MonoScript>();
        foreach (var element in componentCategorySettings)
        {
            foreach (var settings in element.components)
            {
                allCurrentComponents.Add(CreateComponent(settings).component);
            }
        }

        return allCurrentComponents;
    }

    private void UpdateCategories(ComponentCategory element, List<ComponentCategories> currentCategories)
    {
        var isAllCategory = element.category == ComponentCategories.All;
        var containsCategory = currentCategories.Contains(element.category);

        if (containsCategory || isAllCategory)
        {
            var elementIndex = currentCategories.IndexOf(element.category);
            var newOtherComponents = element.components;

            if (isAllCategory)
            {
                var oldCategory = element.components[0].category;
                _allCategories.Remove(oldCategory);
                AddCategory(oldCategory, newOtherComponents.ToArray());
                currentCategories.Add(oldCategory);
                componentCategorySettings.Remove(element);
            }
            else
            {
                Debug.LogError("Warning: You are trying to change the category of multiple components at once: Blocking Action - Sorting Collection...");
                var oldCategoryComponents = componentCategorySettings[elementIndex].components;
                var oldCategory = oldCategoryComponents[0].category;

                if (oldCategory == element.category)
                { 
                    componentCategorySettings[elementIndex].components.AddRange(newOtherComponents); 
                    componentCategorySettings.Remove(element);
                    return;
                }
                
                _allCategories.Remove(oldCategory);
                AddCategory(oldCategory, oldCategoryComponents.ToArray());
                componentCategorySettings.RemoveAt(elementIndex);
            }
        }
        else
            currentCategories.Add(element.category);
        
    }

    private List<ComponentCategories> GetCurrentCategories()
    {
        var currentCategories = new List<ComponentCategories>();

        foreach (var element in componentCategorySettings.ToArray())
        {
            if (element.components.IsEmpty())
                componentCategorySettings.Remove(element);
            UpdateCategories(element, currentCategories);
        }

        return currentCategories;
    }

    public void UpdateCollection()
    {
        UpdateList();
        if (componentCategorySettings.IsEmpty()) ClearList();
        var hasOther = false;
        var isAllCategory = false;

        foreach (var element in componentCategorySettings.ToArray())
        {
            if (element.category == ComponentCategories.Other) hasOther = true;
            if (!ContainsCategory(element.category)) _allCategories.Add(element.category);

            var componentsList = element.components.ToArray();
            foreach (var settings in componentsList)
            {
                isAllCategory = settings.category == ComponentCategories.All;
                if (element.category == settings.category) continue;
                
                if (isAllCategory)
                {
                    MoveComponents(settings, element.category);
                    continue;
                }

                AddCategory(settings.category);

                var categoryIndex = GetCategoryIndex(settings.category);

                var categoryContainsSetting = componentCategorySettings[categoryIndex].components.Contains(settings);
                if (categoryContainsSetting) continue;

                componentCategorySettings[categoryIndex].components.Add(CreateComponent(settings));
                element.components.Remove(settings);
            }
        }

        if (!hasOther) AddCategory(ComponentCategories.Other);
    }
}