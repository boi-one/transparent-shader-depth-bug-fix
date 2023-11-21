using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MagneticPlatformOffState : StateBehaviour
{
    [SerializeField] private MagneticPlatform magneticPlatform;
    [SerializeField] private float offDuration;
    [SerializeField] private string triggerName;
    public override void OnEnter()
    {
        base.OnEnter();
        Invoke("TurnOn", offDuration);
    }

    private void TurnOn()
    {
        magneticPlatform.SetStateTrigger(triggerName, true);
    }
}