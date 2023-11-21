#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor.Events;
using UnityEngine;

public class ForceSystemDebugger : GenericSingleton<ForceSystemDebugger>
{
    private Dictionary<ForceBody, ForceBodyDebugInfo> _forceBodyDebugInfoMap = new Dictionary<ForceBody, ForceBodyDebugInfo>();
    
    [SerializeField] private DebugArrow arrowPrefab;
    [SerializeField] private Vector3 arrowOffset;
    [SerializeField] private ForceSystem forceSystem;
    [SerializeField] private Transform debugArrowContainer;
    
    [Tooltip("Dispose timeout of the ForceDebugInfo in milliseconds")]
    [SerializeField] private int forceInfoDisposeTimeout;
    [SerializeField] private float globalScaleModifier = 1f;

    [SerializeField] private SerializableDictionary<DisplaySetting> forceDisplaySettings;

    private bool IsActive => gameObject.activeSelf && enabled;

    private bool _isJustWokenUp;

    private void Awake()
    {
        _isJustWokenUp = true;
    }

    private void OnEnable()
    {
        if (_isJustWokenUp)
        {
            _isJustWokenUp = false;
            return;
        }
        
        InitializeListeners();
    }

    private void OnDisable()
    {
        Reset();
    }

    public void OnForceBodiesChanged(List<ForceBody> bodies)
    {
        if (!IsActive)
        {
            Reset();
            return;
        }
        
        if(_forceBodyDebugInfoMap.Count > 0)
            return;
        
        var l = bodies.Count;
        for (var i = 0; i < l; i++)
            TrackForceBody(bodies[i]);
    }

    public void TrackForceBody(ForceBody targetForceBody)
    {
        if (_forceBodyDebugInfoMap.ContainsKey(targetForceBody))
            return;

        var debugInfo = new ForceBodyDebugInfo(targetForceBody,
            new DebuggerSettings
        {
            debugArrowPrefab = arrowPrefab,
            disposeTimeout = forceInfoDisposeTimeout,
            globalScaleModifier = globalScaleModifier,
            forceIdMap = forceDisplaySettings,
            debugArrowParent = debugArrowContainer,
            arrowOffset = arrowOffset
        });
        _forceBodyDebugInfoMap.Add(targetForceBody, debugInfo);
    }

    public void UnTrackForceBody(ForceBody targetForceBody)
    {
        if (!_forceBodyDebugInfoMap.TryGetValue(targetForceBody, out var debugInfo))
            return;
        
        debugInfo.Dispose();
        _forceBodyDebugInfoMap.Remove(targetForceBody);
    }

    private void RemoveListeners()
    {
        forceSystem.onForceBodiesChange.RemovePersistentListener(OnForceBodiesChanged);
        forceSystem.onForceBodyAdd.RemovePersistentListener(TrackForceBody);
        forceSystem.onForceBodyRemove.RemovePersistentListener(UnTrackForceBody);
    }

    private void Reset()
    {
        RemoveListeners();
        ClearInfoMap();
    }

    private void InitializeListeners()
    {
        UnityEventTools.AddPersistentListener(forceSystem.onForceBodiesChange, OnForceBodiesChanged);
        UnityEventTools.AddPersistentListener(forceSystem.onForceBodyAdd, TrackForceBody);
        UnityEventTools.AddPersistentListener(forceSystem.onForceBodyRemove, UnTrackForceBody);
    }

    private void ClearInfoMap()
    {
        var l = _forceBodyDebugInfoMap.Count;
        for (var i = 0; i < l; i++)
        {
            var pair = _forceBodyDebugInfoMap.ElementAt(0);
            pair.Value.Dispose();
            _forceBodyDebugInfoMap.Remove(pair.Key);
        }
    }
}
#endif