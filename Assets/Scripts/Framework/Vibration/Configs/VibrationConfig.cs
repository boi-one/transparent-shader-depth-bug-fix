using System.Collections.Generic;
using UnityEngine;

public abstract class VibrationConfig {
    private float _leftMotorFrequency = 1.0f;
    private float _rightMotorFrequency = 1.0f;
    private float _vibrationTime = 1.0f;
    private float _currentVibrationTime;

    public float LeftMotorFrequency
    {
        get => _leftMotorFrequency;
        set => _leftMotorFrequency = value;
    }

    public float RightMotorFrequency
    {
        get => _rightMotorFrequency;
        set => _rightMotorFrequency = value;
    }

    public float VibrationTime
    {
        get => _vibrationTime;
        set => _vibrationTime = value;
    }

    public float CurrentVibrationTime
    {
        get => _currentVibrationTime;
        set => _currentVibrationTime = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public EventData EventData
    {
        get => _eventData;
        set => _eventData = value;
    }

    private bool _isActive = false;
    
    private EventData _eventData;
    
    public void UpdateConfigTime(float elapsedTime)
    {
        _currentVibrationTime -= elapsedTime;
    }

    public bool IsConfigOverDue()
    {
        return _currentVibrationTime <= 0;
    }
    

    public abstract void Initialize();
}
