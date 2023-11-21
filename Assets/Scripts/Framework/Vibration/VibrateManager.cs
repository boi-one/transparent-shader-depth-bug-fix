using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VibrateManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onVibration = new UnityEvent();

    private List<VibrationConfig> _activeVibrations = new List<VibrationConfig>();

    private Dictionary<EventTypes, VibrationConfig> _vibrationConfigs = new Dictionary<EventTypes, VibrationConfig>();

    [SerializeField] private bool isEnabled = true;

    private void Start()
    {

    }

    private void Update()
    {

        if (Gamepad.all.Count == 0 || !isEnabled) return;

        if (!HasActiveVibrations())
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                Gamepad gamepad = Gamepad.all[i];
                gamepad.SetMotorSpeeds(0, 0);
            }
            return;
        } 
        
        CallVibrateEvent(); 
        float elapsedTime = Time.deltaTime;

        GetOverDueVibrationConfigs().ForEach(vibrationConfig => RemoveConfig(vibrationConfig));

        foreach (Gamepad gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(GetLeftMotorFrequency(), GetRightMotorFrequency());
        }
        
        _activeVibrations.ForEach(vibrationConfig => vibrationConfig.UpdateConfigTime(elapsedTime));
    }

    public void TriggerVibration(EventTypes eventType)
    {
        VibrationConfig vibrationConfig = GetVibrationConfigByEventType(eventType);

        if (vibrationConfig == null) return;

        InitializeVibrationConfig(vibrationConfig);

        VibrationConfig settings = CreateVibrationConfig(vibrationConfig);
        
        _activeVibrations.Add(settings);
    }

    public void TriggerVibration(string eventType)
    {
        EventTypes eventTypes;
        if (!EventTypes.TryParse(eventType, true, out eventTypes))
        {
            Debug.LogErrorFormat("Couldnt find eventtype: " + eventType);
            return;
        }
        TriggerVibration(eventTypes);
    }

    public void TriggerVibration(EventData eventData)
    {
        VibrationConfig vibrationConfig = GetVibrationConfigByEventType(eventData.GetEventType());

        if (vibrationConfig == null) return;

        InitializeVibrationConfig(vibrationConfig, eventData);

        VibrationConfig settings = CreateVibrationConfig(vibrationConfig);
        
        _activeVibrations.Add(settings);
    }

    public void AddVibrationConfig(EventTypes eventType, VibrationConfig vibrationConfig)
    {
        _vibrationConfigs.Add(eventType, vibrationConfig);
    }

    private bool HasActiveVibrations()
    {
        return _activeVibrations.Count >= 1;
    }

    private VibrationConfig CreateVibrationConfig(VibrationConfig vibrationConfig)
    {
        BaseConfig baseConfig = new BaseConfig();
        baseConfig.Initialize();

        baseConfig.LeftMotorFrequency = vibrationConfig.LeftMotorFrequency;
        baseConfig.RightMotorFrequency = vibrationConfig.RightMotorFrequency;
        baseConfig.CurrentVibrationTime = vibrationConfig.CurrentVibrationTime;
        baseConfig.VibrationTime = vibrationConfig.VibrationTime;

        if (vibrationConfig.EventData != null)
        {
            baseConfig.EventData = vibrationConfig.EventData;
        }
        
        return baseConfig;
    }
    
    private VibrationConfig GetVibrationConfigByEventType(EventTypes eventType)
    {
        VibrationConfig vibrationConfig;
        _vibrationConfigs.TryGetValue(eventType, out vibrationConfig);
        return vibrationConfig;
    }

    private void InitializeVibrationConfig(VibrationConfig vibrationConfig, EventData eventData)
    {
        vibrationConfig.EventData = eventData;
        vibrationConfig.Initialize();
    }
    
    private void InitializeVibrationConfig(VibrationConfig vibrationConfig)
    {
        vibrationConfig.Initialize();
    }

    private List<VibrationConfig> GetOverDueVibrationConfigs()
    {
        List<VibrationConfig> overDueConfigs = new List<VibrationConfig>();
        for (int i = 0; i < _activeVibrations.Count; i++)
        {
            VibrationConfig vibrationConfig = _activeVibrations[i];
            if (vibrationConfig.IsConfigOverDue())
            {
                overDueConfigs.Add(vibrationConfig);
            }
        }
        return overDueConfigs;
    }

    private float GetLeftMotorFrequency()
    {
        float leftMotorFrequency = 0f;
        
        _activeVibrations.ForEach(vibrationConfig => leftMotorFrequency += vibrationConfig.LeftMotorFrequency);

        return leftMotorFrequency;
    }
    
    private float GetRightMotorFrequency()
    {
        float rightMotorFrequency = 0f;
        
        _activeVibrations.ForEach(vibrationConfig => rightMotorFrequency += vibrationConfig.RightMotorFrequency);

        return rightMotorFrequency;
    }

    private void RemoveConfig(VibrationConfig vibrationConfig)
    {
        _activeVibrations.Remove(vibrationConfig);
    }

    private void CallVibrateEvent()
    {
        onVibration?.Invoke();
    }

}
