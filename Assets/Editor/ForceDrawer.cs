using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer(typeof(Force))]
public class ForceDrawer : PropertyDrawer
{
    private const int HelpBoxFontSize = 13;
    private readonly RectOffset _helpBoxPadding = new RectOffset(20, 20, 10, 10);
    private readonly TextAnchor _helpBoxTextAlignment = TextAnchor.MiddleCenter;
    
    private string _helpBoxText = "";
    private float _height;

    private Dictionary<string, Func<SerializedProperty, SerializedProperty, bool>> _canRenderProperty = new Dictionary<string, Func<SerializedProperty, SerializedProperty, bool>>()
    {
        {
            "curve", (currentProperty, property) => IsCurvedForce(property)
        },
        {
            "duration", (currentProperty, property) => IsCurvedForce(property)
        },
        {
            "delay", (currentProperty, property) => IsTimedForce(property)
        }
    };

    private Dictionary<string, Func<SerializedProperty, string>> _renderHelpBox = new Dictionary<string, Func<SerializedProperty, string>>()
    {
        {
            "type", (property) =>
            {
                var forceType = (ForceTypes)property.intValue;

                return forceType switch
                {
                    ForceTypes.Default => "test default type.",
                    ForceTypes.Collision => "test collision type.",
                    ForceTypes.SingleAddition => "test single addition type.",
                    ForceTypes.Continuous => "test continuous type.",
                    ForceTypes.Gravity => "test gravity type.",
                    _ => null
                };
            }
        },
        {
            "modifier", (property) =>
            {
                var forceModifiers = (ForceModifiers)property.intValue;

                return forceModifiers switch
                {
                    ForceModifiers.None => "test none modifier.",
                    ForceModifiers.Stack => "test stack modifier.",
                    _ => null
                };
            }
        }
    };
    
    private Dictionary<string, Action<SerializedProperty>> _preRenderActions = new Dictionary<string, Action<SerializedProperty>>()
    {
        {
            "priority", (currentProperty) =>
            {
                var priority = (ForcePriority)currentProperty.intValue;

                GUI.color = priority switch
                {
                    ForcePriority.High => Color.red,
                    ForcePriority.Low => Color.green,
                    _ => GUI.color
                };
            }
        }
    };

    private Dictionary<string, Action<SerializedProperty>> _postRenderActions = new Dictionary<string, Action<SerializedProperty>>()
    {
        {"priority", (property) => { GUI.color = Color.white; }}
    };
    
    private static bool IsCurvedForce(SerializedProperty property)
    {
        return IsOfType(property, ForceTypes.Default);
    }
    
    private static bool IsTimedForce(SerializedProperty property)
    {
        return IsOfType(property, ForceTypes.Default, ForceTypes.SingleAddition);
    }

    private static bool IsOfType(SerializedProperty property, params ForceTypes[] targetTypes)
    {
        var typeField = property.FindPropertyRelative("type");
        var forceType = (ForceTypes)typeField.intValue;
        return Array.Exists(targetTypes, element => element == forceType);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = 0f;
        var helpBoxText = "";

        if (!property.isExpanded) return base.GetPropertyHeight(property, label);
        
        //Calculate property height
        typeof(Force).ForEachProperty(targetProperty =>
        {
            var backingFieldName = GetBackingField(targetProperty);
            if (backingFieldName == null) return;
            
            var currentProperty = property.FindPropertyRelative(backingFieldName.Name);
            if (currentProperty == null) return;
            
            if (_canRenderProperty.TryGetValue(currentProperty.name, out var action))
            {
                if (!action.Invoke(currentProperty, property)) return;
            }
            
            UpdateHeight(ref height, EditorGUI.GetPropertyHeight(currentProperty, true));
            
            if (_renderHelpBox.TryGetValue(currentProperty.name, out var renderHelpBoxAction))
            {
                AddHelpBoxText(ref helpBoxText, renderHelpBoxAction.Invoke(currentProperty));
            }
        });
        
        //Calculate help box height
        var helpBoxContent = new GUIContent(helpBoxText);
        UpdateHeight(ref height, GetHelpBoxHeight(helpBoxContent, EditorGUIUtility.currentViewWidth));

        return base.GetPropertyHeight(property, label) + height;
    }

    private FieldInfo GetBackingField(PropertyInfo propertyInfo)
    {
        var getMethod = propertyInfo.GetGetMethod();
        if (getMethod == null) return null;
        
        var methodBody = getMethod.GetMethodBody();
        if (methodBody == null) return null;
        
        var fieldReference = BitConverter.ToInt32(methodBody.GetILAsByteArray(), 2);
        return (FieldInfo) propertyInfo.DeclaringType?.Module.ResolveMember(fieldReference);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Reset();
        
        EditorGUI.BeginProperty(position, label, property);

        //Render label
        var labelHeight = EditorGUIUtility.singleLineHeight;
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, labelHeight), property.isExpanded, label);
        
        //Check if label is closed
        if (property.isExpanded)
        {
            UpdateHeight(ref _height, position.y + labelHeight);
        
            var previousIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = previousIndentLevel + 1;

            RenderAllProperties(position, property);
        
            UpdateHeight(ref _height, RenderHelpBox(position.x, _height, position.width, _helpBoxText));

            EditorGUI.indentLevel = previousIndentLevel;
        }

        EditorGUI.EndProperty();
    }

    private void RenderAllProperties(Rect position, SerializedProperty property)
    {
        typeof(Force).ForEachProperty(targetProperty =>
        {
            var backingFieldName = GetBackingField(targetProperty);
            if (backingFieldName == null) return;
            
            var currentProperty = property.FindPropertyRelative(backingFieldName.Name);
            if (currentProperty == null) return;
            
            //Check if property can render
            if (_canRenderProperty.TryGetValue(currentProperty.name, out var action))
            {
                if (!action.Invoke(currentProperty, property)) return;
            }

            //Run pre render actions
            if (_preRenderActions.TryGetValue(currentProperty.name, out var preAction)) preAction.Invoke(currentProperty);

            //Render property
            UpdateHeight(ref _height, RenderProperty(position.x, _height, position.width, currentProperty));

            //Render help box
            if (_renderHelpBox.TryGetValue(currentProperty.name, out var renderHelpBoxAction))
            {
                AddHelpBoxText(ref _helpBoxText, renderHelpBoxAction.Invoke(currentProperty));
            }

            //Run post actions
            if(_postRenderActions.TryGetValue(currentProperty.name, out var postAction)) postAction.Invoke(currentProperty);
        });
    }

    private void UpdateHeight(ref float fullHeight, float height)
    {
        fullHeight += height + EditorGUIUtility.standardVerticalSpacing;
    }

    private void Reset()
    {
        _height = 0;
        _helpBoxText = "";
    }

    private void AddHelpBoxText(ref string allText, string text)
    {
        if (text?.Length == 0) return;

        if (allText.Length > 0) allText += "\n";
        
        allText += text;
    }

    private float RenderProperty(float positionX, float positionY, float width, SerializedProperty property)
    {
        var propertyHeight = EditorGUI.GetPropertyHeight(property, true);
        EditorGUI.PropertyField(new Rect(positionX, positionY, width, propertyHeight), property);

        return propertyHeight;
    }

    private float RenderHelpBox(float positionX, float positionY, float width, string text)
    {
        var helpBoxContent = new GUIContent(text);

        float height = GetHelpBoxHeight(helpBoxContent, width);
        
        EditorGUI.HelpBox(new Rect(positionX, positionY, width, height), text, MessageType.None);

        return height;
    }

    private float GetHelpBoxHeight(GUIContent content, float width)
    {
        var helpBoxStyle = GUI.skin.GetStyle("HelpBox");
        helpBoxStyle.fontSize = HelpBoxFontSize;
        helpBoxStyle.padding = _helpBoxPadding;
        helpBoxStyle.alignment = _helpBoxTextAlignment;

        return helpBoxStyle.CalcHeight(content, width);
    }
}
