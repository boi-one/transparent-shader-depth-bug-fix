using System;
using UnityEngine;

[Serializable]
public class MovementSettings
{
    public MovementStates id = MovementStates.Walking;
    public float groundSpeed = 1f;
    public float airSpeed = 0.8f;
    public float maxGroundAcceleration = 100f;

    public static MovementSettings operator /(MovementSettings targetSetting, float divider)
    {
        if (divider == 0)
        {
            throw new DivideByZeroException();
        }
        targetSetting.groundSpeed /= divider;
        targetSetting.airSpeed /= divider;
        targetSetting.maxGroundAcceleration /= divider;
        return targetSetting;
    }
    
    public static MovementSettings operator *(MovementSettings targetSetting, float multiplier)
    {
        targetSetting.groundSpeed *= multiplier;
        targetSetting.airSpeed *= multiplier;
        targetSetting.maxGroundAcceleration *= multiplier;
        return targetSetting;
    }

}
