using System;
using System.Collections.Generic;

public class ForceBodyDebugInfo : IDisposable
{
    private bool _isDisposed;
    private readonly ForceBody _forceBody;
    private readonly DebuggerSettings _settings;

    public ForceBody ForceBody => _forceBody;

    private readonly ForceBodyDebugInfoContainer _mapDebugInfo = new ForceBodyDebugInfoContainer();

    public ForceBodyDebugInfo(ForceBody forceBody, DebuggerSettings settings)
    {
        _forceBody = forceBody;
        _settings = settings;
        InitializeListeners();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        
        RemoveListeners();
        ClearInfo(); 
        
        _isDisposed = true;
    }

    public ForceDebugInfo GetForceInfo(IForceable force)
    {
        var output = _mapDebugInfo.GetDebugInfo(force);
        return output?.debugInfo; 
    }

    private void ClearInfo()
    {
        _mapDebugInfo.Empty();
    }

    private void InitializeListeners()
    {
        _forceBody.onForceAdded.AddListener(OnForceAdded);
        _forceBody.onForceRemoved.AddListener(OnForceRemoved);
        _forceBody.onForcesConcatenated.AddListener(OnForcesConcatenated);
        _forceBody.onForcesCleared.AddListener(OnForcesCleared);
    }

    private void RemoveListeners()
    {
       _forceBody.onForceAdded.RemoveListener(OnForceAdded); 
       _forceBody.onForceRemoved.RemoveListener(OnForceRemoved);
       _forceBody.onForcesConcatenated.RemoveListener(OnForcesConcatenated);
       _forceBody.onForcesCleared.RemoveListener(OnForcesCleared);
    }

    private void OnForceAdded(IForceable force, List<IForceable> targetCollection)
    {
        var debugInfo = new ForceDebugInfo(this, force, _settings);
        _mapDebugInfo.Add(targetCollection, force, debugInfo);
    }

    private void OnForceRemoved(IForceable force, List<IForceable> targetCollection)
    {
        _mapDebugInfo.Remove(targetCollection, force);
    }

    private void OnForcesConcatenated(List<IForceable> @base, List<IForceable> toAppend)
    {
        _mapDebugInfo.Concat(@base, toAppend);
    }

    private void OnForcesCleared(List<IForceable>[] targetCollections)
    {
       _mapDebugInfo.Clear(targetCollections); 
    }
}
