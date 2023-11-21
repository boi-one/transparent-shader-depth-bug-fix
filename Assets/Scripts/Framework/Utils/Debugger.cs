using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Debugger : GenericSingleton<Debugger>
{
    [Serializable]
    public struct BoolSetting
    {
        public string Key;
        public bool Value;
    }

    [SerializeField] private bool _pauseByDefault = false;
    [SerializeField] private bool _logByDefault = true;
    
    [SerializeField] private List<BoolSetting> pauseSettings = new List<BoolSetting>(); 
    [SerializeField] private List<BoolSetting> logSettings = new List<BoolSetting>(); 

    public static void Log(string key, object message)
    {
        if (!Contains(Instance.logSettings, key))
        {
            Instance.logSettings.Add(new BoolSetting()
            {
                Key = key,
                Value = Instance._logByDefault
            });
        }
        
        if(IsTrue(Instance.logSettings, key))
            Debug.Log(message);
    }
    
    public static void LogIf(bool useLog, object message)
    {
        if(useLog) Debug.Log(message);
    }

    public static void Break(string key)
    {
        if (!Contains(Instance.pauseSettings, key))
        {
            Instance.pauseSettings.Add(new BoolSetting()
            {
                Key = key,
                Value = Instance._pauseByDefault
            });
        }

        if(IsTrue(Instance.pauseSettings, key))
            Debug.Break();

    }
    
    public static void BreakIf(bool useBreak, string key)
    {
        if (useBreak) Break(key);
    }

    private static bool Contains(List<BoolSetting> targetList, string key)
    {
        return targetList.Exists(item => item.Key == key);
    }

    private static bool IsTrue(List<BoolSetting> targetList, string key)
    {
        if (!Contains(targetList, key))
            return false;
        
        var targetSetting = targetList.Find(item => item.Key == key);
        return targetSetting.Value;
    }
}
