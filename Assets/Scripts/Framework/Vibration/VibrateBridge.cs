using System;
using UnityEngine;
public class VibrateBridge : MonoBehaviour
{

    private VibrateManager _vibrateManager;
    
    private void Start()
    {
        _vibrateManager = GameObject.FindObjectOfType<VibrateManager>();
    }

    public void TriggerVibration(string type)
    {
        _vibrateManager.TriggerVibration(type);
    }

    public void TriggerVibration(EventData eventData)
    {
        _vibrateManager.TriggerVibration(eventData);
    }
}
