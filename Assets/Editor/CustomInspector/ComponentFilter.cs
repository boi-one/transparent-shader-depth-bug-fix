using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObject))]
public class ComponentFilter : Editor
{
    // Static Variables
    private static int _selectedLabelIndex;
    private static int _previousLabelIndex;

    private static string _searchText = "";
    private static string _previousSearchText = "";

    private static bool _showComponentFilter = true;

    private static string _nameComponent;
    private static string _previousNameComponent;

    private static StaticPopupWindow _staticPopupWindow;

    // Dynamic Variables
    private string[] _labelNames;
    private const string CustomInspectorName = "Component Filter";

    private int _maxLabelsPerRow = 3;
    private int _screenLabelSpacing = Screen.width + 100;

    private GameObject _targetGameObject;
    private GameObject _previousTargetGameObject;

    private List<string> _filteredComponentList = new List<string>();
    private readonly List<string> _gizmosIconMenuOptions = new List<string>();
    private readonly List<string> _shownCategories = new List<string>();

    private readonly GUIStyle _buttonStyleGizmosIcon = new GUIStyle();

    private readonly Rect _gizmoIconRect = new Rect(10, 4, 32, 39);
    private Rect _staticPopupWindowRect;

    private Vector2 _centerofScreen;
    private readonly Vector2 _windowSize = new Vector2(450, 150);

    private ComponentCategoriser _componentCategoriser;

    private bool IsTargetNull => ReferenceEquals(_targetGameObject, null);

    // Unity Internal Functions
    private void OnEnable()
    {
        InitComponentCategoriser();
        UpdateLabelGrid();

        _componentCategoriser.RefreshComponents(_targetGameObject, true);
        _labelNames = GetLabelNames();
    }

    protected override void OnHeaderGUI()
    {
        SetTargetAsGameObject();
    }

    public override void OnInspectorGUI()
    {
        DrawBaseInspector();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        _showComponentFilter = EditorGUILayout.Foldout(_showComponentFilter, CustomInspectorName);

        if (!_showComponentFilter) return;
        
        if (_targetGameObject != null) _nameComponent = _targetGameObject.name;

        UpdateLabelGrid();
        RenderSearchBar();
        RenderLabelGrid();

        var hasSelectedNewGameObject = _nameComponent != _previousNameComponent;
        if (hasSelectedNewGameObject) ResetInspector();

        var hasCurrentObjectChanged = _targetGameObject != _previousTargetGameObject;
        if (hasCurrentObjectChanged)
        {
            _previousTargetGameObject = _targetGameObject;
            RefreshInspectorWindow();
        }

        if (_searchText != _previousSearchText) RefreshInspectorWindow();

        var hasIndexChanged = _previousLabelIndex == _selectedLabelIndex;
        if (hasIndexChanged) return;
        
        _labelNames = GetLabelNames();

        _previousLabelIndex = _selectedLabelIndex;

        RefreshInspectorWindow();
    }

    // OUR Functions

    private void ResetInspector()
    {
        _selectedLabelIndex = 0;
        _previousLabelIndex = 0;
        _previousNameComponent = _nameComponent;
        _componentCategoriser.SaveAllComponents(_targetGameObject);
    }

    private void RefreshInspectorWindow()
    {
        FilterList();
        HideComponents();
        EditorUtility.SetDirty(target);
    }

    private void InitComponentCategoriser()
    {
        _componentCategoriser = new ComponentCategoriser();
    }

    private void SetTargetAsGameObject()
    {
        _targetGameObject = target as GameObject;
    }

    private void RenderLabelGrid()
    {
        EditorGUILayout.BeginHorizontal();
        _selectedLabelIndex = GUILayout.SelectionGrid(_selectedLabelIndex, _labelNames, _maxLabelsPerRow);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    private static void AddGizmosIconOptions(List<string> iconMenuOptions)
    {
        // Experimental
        iconMenuOptions.Add(" ");
        iconMenuOptions.Add("Color1");
        iconMenuOptions.Add("Color2");
        // !Experimental

        // Add Icon Menu here
    }

    private static string[] FillDropdownList(params string[] fillerList)
    {
        return fillerList;
    }

    private Vector2 GetCenterOfScreen()
    {
        _centerofScreen.x = Screen.currentResolution.width * 0.5f - _staticPopupWindow.minSize.x;
        _centerofScreen.y = Screen.currentResolution.height * 0.5f - _staticPopupWindow.minSize.y;

        return _centerofScreen;
    }

    public void DrawBaseInspector()
    {
        if (IsTargetNull) SetTargetAsGameObject();

        var targetLayer = _targetGameObject.layer;
        var isTargetActive = _targetGameObject.activeSelf;
        var targetName = _targetGameObject.name;
        var targetCurrentTag = _targetGameObject.tag;
        var isTargetStatic = _targetGameObject.isStatic;

        var openStaticPopUp = isTargetStatic;
        var staticDropdownOptions = FillDropdownList("Nothing", "[WIP]");

        var gizmosIconMenuIndex = 0;
        _gizmosIconMenuOptions.Clear();

        var targetIconTexture = PrefabUtility.GetIconForGameObject(_targetGameObject);

        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(-3);

        AddGizmosIconOptions(_gizmosIconMenuOptions);
        gizmosIconMenuIndex = EditorGUILayout.Popup(gizmosIconMenuIndex, _gizmosIconMenuOptions.ToArray(),
            _buttonStyleGizmosIcon, GUILayout.Width(30), GUILayout.Height(27));
        GUI.DrawTexture(_gizmoIconRect, targetIconTexture, ScaleMode.ScaleToFit, true, 1);

        GUILayout.Space(10);

        isTargetActive = GUILayout.Toggle(isTargetActive, string.Empty, GUILayout.Width(15));
        targetName = GUILayout.TextField(targetName);
        openStaticPopUp = GUILayout.Toggle(isTargetStatic, "Static", GUILayout.Width(50));

        var staticDropdownIndex = staticDropdownOptions.Length;
        staticDropdownIndex =
            EditorGUILayout.Popup(staticDropdownIndex, staticDropdownOptions.ToArray(), GUILayout.Width(20));

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(-6);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(35);

        GUILayout.Label("Tag", GUILayout.Width(30));
        targetCurrentTag = EditorGUILayout.TagField(targetCurrentTag);
        GUILayout.Label("Layer", GUILayout.Width(40));
        targetLayer = EditorGUILayout.LayerField(targetLayer);

        EditorGUILayout.EndHorizontal();

        if (_targetGameObject.activeSelf != isTargetActive)
            _targetGameObject.SetActive(isTargetActive);
        if (_targetGameObject.name != targetName)
            _targetGameObject.name = targetName;
        if (_targetGameObject.layer != targetLayer)
            _targetGameObject.layer = targetLayer;
        if (!_targetGameObject.CompareTag(targetCurrentTag))
            _targetGameObject.tag = targetCurrentTag;
        if (_targetGameObject.isStatic != isTargetStatic)
            _targetGameObject.isStatic = isTargetStatic;

        var hasChangedIsTargetStatic = openStaticPopUp != isTargetStatic;
        if (hasChangedIsTargetStatic && _staticPopupWindow == null)
        {
            _staticPopupWindow = CreateInstance(typeof(StaticPopupWindow)) as StaticPopupWindow;
            _staticPopupWindow!.targetObject = _targetGameObject;

            _staticPopupWindow.minSize = _windowSize;
            _staticPopupWindow.maxSize = _windowSize;

            _staticPopupWindowRect.x = GetCenterOfScreen().x;
            _staticPopupWindowRect.y = GetCenterOfScreen().y;
            _staticPopupWindow.ShowUtility();
            _staticPopupWindow.position = _staticPopupWindowRect;
        }

        var prefabObject = PrefabUtility.GetCorrespondingObjectFromSource(_targetGameObject);
        var isPrefab = prefabObject != null;
        if (!isPrefab) return;

        var openAndSelectButtonsText = FillDropdownList("Open", "Select");
        var openAndSelectButtonsIndex = 0;

        var overrideDropdownOptions = FillDropdownList("Overrides", " ", "Apply All", "Revert All");
        var overrideDropdownIndex = 0;

        var prefabAssetPath = AssetDatabase.GetAssetOrScenePath(prefabObject);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(2);
        GUILayout.Label("Prefab", GUILayout.Width(41));

        openAndSelectButtonsIndex = GUILayout.Toolbar(openAndSelectButtonsText.Length, openAndSelectButtonsText);
        overrideDropdownIndex = EditorGUILayout.Popup(overrideDropdownIndex, overrideDropdownOptions.ToArray());

        EditorGUILayout.EndHorizontal();

        if (overrideDropdownOptions[overrideDropdownIndex] == "Apply All")
            PrefabUtility.ApplyPrefabInstance(_targetGameObject, InteractionMode.UserAction);

        if (overrideDropdownOptions[overrideDropdownIndex] == "Revert All")
            PrefabUtility.RevertPrefabInstance(_targetGameObject, InteractionMode.UserAction);

        if (openAndSelectButtonsIndex == openAndSelectButtonsText.Length) return;

        var prefabGameObject = prefabObject as GameObject;
        var childObject = prefabGameObject.GetComponentInChildren<MonoBehaviour>();

        if (openAndSelectButtonsText[openAndSelectButtonsIndex] == "Open")
        {
            if (childObject != null)
            {
                var childPrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabGameObject);

                AssetDatabase.OpenAsset(childPrefab != null ? childPrefab : prefabObject);
            }
            else
            {
                AssetDatabase.OpenAsset(prefabObject);
            }
        }
        else if (openAndSelectButtonsText[openAndSelectButtonsIndex] == "Select")
        {
            if (childObject != null)
            {
                var childPrefab = PrefabUtility.GetCorrespondingObjectFromSource(childObject);

                if (childPrefab != null)
                    prefabAssetPath = AssetDatabase.GetAssetOrScenePath(childPrefab);
            }

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
            EditorGUIUtility.PingObject(obj);
        }
    }

    private void FilterList()
    {
        _componentCategoriser.RefreshComponents(_targetGameObject);
        _labelNames = GetLabelNames();
        _filteredComponentList = _componentCategoriser.GetTargetComponentNames(_labelNames[_selectedLabelIndex], _searchText);

        if (_searchText != "") return;
        _filteredComponentList = _componentCategoriser.GetTargetComponentNames(_labelNames[_selectedLabelIndex]);
    }

    private void HideComponents()
    {
        var targetComponents = _targetGameObject.GetComponents<Component>();
        foreach (var component in targetComponents)
        {
            var containsComponent = _filteredComponentList.Contains(component.GetType().ToString());

            if (containsComponent)
                component.hideFlags = _targetGameObject.hideFlags & ~HideFlags.HideInInspector;
            else
                component.hideFlags = _targetGameObject.hideFlags | HideFlags.HideInInspector;
        }
    }

    private void RenderSearchBar()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Filter Components:");

        _previousSearchText = _searchText;
        _searchText = GUILayout.TextField(_searchText, 25, GUILayout.Width(150));
        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    private string[] GetLabelNames()
    {
        var allComponents = _componentCategoriser.GetTargetComponentNames(ComponentCategories.All.ToString());
        var componentDatabase = _componentCategoriser.currentCategories;
        _shownCategories.Clear();

        _shownCategories.Add(ComponentCategories.All.ToString());

        foreach (var script in componentDatabase.Keys)
        {
            var category = componentDatabase[script];
            var containsScript = allComponents.Contains(script.ToString());
            if (!containsScript || category == ComponentCategories.Other) continue;

            var categoryName = category.ToString();

            if (_shownCategories.Contains(categoryName)) continue;
            _shownCategories.Add(categoryName);
        }

        _shownCategories.Add(ComponentCategories.Other.ToString());
        return _shownCategories.ToArray();
    }

    private void UpdateLabelGrid()
    {
        var scale = Screen.width;
        var modulo = scale % 100;

        if (scale <= 100) _screenLabelSpacing = 100;
        else if (scale > _screenLabelSpacing) _screenLabelSpacing += 100;
        else if (scale < _screenLabelSpacing - 100) _screenLabelSpacing -= 100;

        if (modulo < 50) _screenLabelSpacing -= 100;

        _maxLabelsPerRow = _screenLabelSpacing / 100;
    }
}