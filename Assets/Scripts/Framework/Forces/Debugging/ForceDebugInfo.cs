using System;
using Object = UnityEngine.Object;

public class ForceDebugInfo : IDisposable
{
    private bool _isDisposed;
    private DebugArrow _debugArrow;
    private readonly int _disposeTimeout;

    public IForceable Force { get; }
    public ForceBodyDebugInfo BodyDebugInfo { get; }

    public ForceDebugInfo(ForceBodyDebugInfo parentInfo, IForceable force, DebuggerSettings settings)
    {
        BodyDebugInfo = parentInfo;
        Force = force;
        _disposeTimeout = settings.disposeTimeout;
        CreateArrow(settings);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Object.Destroy(_debugArrow.gameObject, _disposeTimeout / 1000f);

        _isDisposed = true;
    }

    private void CreateArrow(DebuggerSettings settings)
    {
        _debugArrow = Object.Instantiate(settings.debugArrowPrefab, settings.debugArrowParent);
        _debugArrow.DebugInfo = this;
        _debugArrow.ScaleModifier = settings.globalScaleModifier;
        _debugArrow.Offset = settings.arrowOffset;

        if (settings.forceIdMap.TryGetValue(Force.Id.ToString(), out var info))
        {
            _debugArrow.Color = info.color;
            _debugArrow.ScaleModifier = info.scaleMultiplier * settings.globalScaleModifier;
        }
    }
}
