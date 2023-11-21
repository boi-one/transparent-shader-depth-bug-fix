
using System.Collections.Generic;
using UnityEngine;

public struct DebuggerSettings
{
    public DebugArrow debugArrowPrefab;
    public int disposeTimeout;
    public float globalScaleModifier;
    public Dictionary<string, DisplaySetting> forceIdMap;
    public Transform debugArrowParent;
    public Vector3 arrowOffset;
}
