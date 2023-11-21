using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshakeSystem : GenericSingleton<ScreenshakeSystem>
{
    [Serializable]
    public struct ScreenshakeSettings
    {
        [Tooltip("The name of the preset")]
        public string name;
        [Tooltip("The intensity of the screen shake")]
        [Range(0, 10)]
        public float intensity;
        [Tooltip("The frequency of the shake")]
        [Range(0, 2)]
        public float frequency;
        [Tooltip("The duration of the screen shake")]
        [Range(0, 2)]
        public float duration;
    }

    [SerializeField] private ScreenshakeSettings[] screenshakeSettings;
    private Dictionary<string, ScreenshakeSettings> _settings = new Dictionary<string, ScreenshakeSettings>();

    [SerializeField] private CameraShaker cameraShaker;

    private void Awake()
    {
        foreach (var screenshakeSetting in screenshakeSettings)
        {
            _settings.Add(screenshakeSetting.name, screenshakeSetting);
        }
    }

    public void Shake(string type)
    {
        if (!_settings.ContainsKey(type)) return;

        var currentShake = _settings[type];

        cameraShaker.ShakeCamera(currentShake.intensity, currentShake.duration, currentShake.frequency);
    }
}
