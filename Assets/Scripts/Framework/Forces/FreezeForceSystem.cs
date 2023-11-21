using System;
using UnityEngine;

public class FreezeForceSystem : GenericSingleton<FreezeForceSystem>
{
    [Serializable]
    public struct FreezeForceSettings
    {
        public string name;
        public float delay;
        public float duration;
    }

    [SerializeField] private SerializableDictionary<FreezeForceSettings> freezeForceSettings;

    public void AddFreeze(string type, params GameObject[] targets)
    {
        if (!freezeForceSettings.ContainsKey(type)) return;

        var currentSetting = freezeForceSettings[type];

        var _freezeForce = new Force
        {
            Duration = currentSetting.duration,
            Delay = currentSetting.delay,
            BlendType = BlendTypes.PauseOthers,
            DisablesGravity = true
        };

        var l = targets.Length;
        for (int i = 0; i < l; i++)
        {
            if (!targets[i].TryGetComponent<ForceBody>(out ForceBody forceBody)) continue;
            
            forceBody.Add(_freezeForce);
        }
    }
}
