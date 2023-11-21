using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TimedRotater : Rotater2D
{
    private float _duration;
    private float _startTime;
    
    public void EnableRotater(float newDuration)
    {
        _duration = newDuration;
        EnableRotater();
    }

    public void EnableRotater(float newDuration, float startDelay)
    {
        _duration = newDuration;
        Invoke("EnableRotater", startDelay);
    }

    private void EnableRotater()
    {
        _startTime = Time.time;
        IsEnabled = true;
    }

    private protected override void Update()
    {
        base.Update();
        
        if (!IsEnabled) return;
        
        if (Time.time > _startTime + _duration) IsEnabled = false;
    }
}
