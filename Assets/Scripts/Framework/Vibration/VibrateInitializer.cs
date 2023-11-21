using System;
using Extensions;
using UnityEngine;
using Vibration.Configs.ConfigObjects;

public class VibrateInitializer : MonoBehaviour
{
   [SerializeField] private VibrateManager vibrateManager;
    private void Start()
    {

        if (gameObject.HasComponent<VibrateManager>() && vibrateManager == null)
        {
            vibrateManager = GetComponent<VibrateManager>();
        } else if (!gameObject.HasComponent<VibrateManager>() && vibrateManager == null)
        {
            Debug.LogErrorFormat("VibrateManger isn't initialized: " + gameObject.name);
            return;
        }

        vibrateManager.AddVibrationConfig(EventTypes.PUSH, new FlatVibrationConfig(0.4f, 0.4f, 1.5f));
        vibrateManager.AddVibrationConfig(EventTypes.PULL, new FlatVibrationConfig(0.5f, 0.5f, 0.35f));
        vibrateManager.AddVibrationConfig(EventTypes.MINOR_IMPACT, new FlatVibrationConfig(0.5f, 0.5f, 0.1f));
    }
}
