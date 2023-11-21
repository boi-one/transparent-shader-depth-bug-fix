using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PushVibrationConfig : VibrationConfig
{
    private float _leftMotorFrequency = 0.4f;
    private float _rightMotorFrequency = 0.4f;
    private float _vibrationTime = 1.5f;

    public override void Initialize()
    {
        float modifier = 1f;
        
        if (EventData == null)
        {
            Debug.LogErrorFormat("PushData isn't initialized, the push vibration will not support modifiers. To fix this call the TriggerVibration with EventData.");
        }
        else
        {
            PushData pushData = (PushData) EventData;
            modifier = pushData.GetPushCount();
        }
        

        LeftMotorFrequency = _leftMotorFrequency * modifier;
        RightMotorFrequency = _rightMotorFrequency * modifier;
        VibrationTime = _vibrationTime;
        CurrentVibrationTime = VibrationTime;

        EventData = null;
    }
}