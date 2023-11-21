using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForceLibrary : GenericSingleton<ForceLibrary>
{
    [Serializable]
    public struct ImpactSettings
    {
        public string name;
        public Force impact;
        public bool isDirectionalImpact;
    }
    
    [SerializeField] private ImpactSettings[] impactSettings;
    private Dictionary<string, ImpactSettings> _forces = new Dictionary<string, ImpactSettings>();

    void Start()
    {
        foreach (var impactSetting in impactSettings)
        {
            _forces.Add(impactSetting.name, impactSetting);
        }
    }

    public static Force Get(string impactName)
    {
        if (!Instance._forces.ContainsKey(impactName))
            return null;
        
        return Instance._forces[impactName].impact.Clone();
    }
    
    public static ImpactSettings GetSettings(string impactName)
    {
        if (!Instance._forces.ContainsKey(impactName))
            return new ImpactSettings();
        
        return Instance._forces[impactName];
    }

}
