using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class StaminaLookup : MonoBehaviour
{
    [SerializeField] private List<Actions> actions;
    private readonly Dictionary<string, Actions> _actionsMap = new Dictionary<string, Actions>();

    private StaminaPool _staminaPool;

    private void Start()
    {
        if (actions == null) return;
        _staminaPool = GetComponent<StaminaPool>();
        foreach (var action in actions)
        {
            _actionsMap.Add(action.name, action);
        }
    }
    
    public bool UseStamina(string actionName) // Mark todo: Remove this once speed works with speed and not just bool
    {
        if (!ValidateAction(actionName)) return false;

        DepleteStamina(actionName);
        return true;
    }
    
    public void UseStamina(string actionName, Func<InputAction.CallbackContext, bool> onActionValid, InputAction.CallbackContext context, Action<InputAction.CallbackContext> onActionInValid = null)
    {
        if (!ValidateAction(actionName))
        {
            onActionInValid?.Invoke(context);
            return;
        }
        bool isUsed = onActionValid(context);
        if (!isUsed) return;
        
        DepleteStamina(actionName);
    }

    public bool ValidateAction(string actionName) // Mark Todo: Could also make this enums
    {
        if (actions == null) return false;
        if (_staminaPool == null) return false;
        if (!_actionsMap.ContainsKey(actionName)) return true;
        var action = _actionsMap[actionName];
        return _staminaPool.Stamina - action.staminaCost >= 0;
    }

    public void DepleteStamina(string actionName)
    {
        if (!_actionsMap.ContainsKey(actionName)) return;
        var action = _actionsMap[actionName];
        _staminaPool.DepleteStamina(action.staminaCost);
    }
}

[Serializable]
public struct Actions
{
    public string name;
    public float staminaCost;

    [Header("Events")]
    public UnityEvent onActionValid;
    public UnityEvent onBlockAction;
}