using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class ForceQueue
{
    private List<Force> _forceQueue = new List<Force>();
    private bool _isStarted;

    public Action<Force> OnNext;
    public Action OnFinish;
    public Action OnCancel;

    public  bool IsPersistent { get; set; }
    public bool IsAutoStarting { get; set; }

    public ForceBody TargetForceBody { get; set; }
    
    public ForceQueue(ForceBody forceBody, bool isPersistent = false, bool isAutoStarting = false)
    {
        IsPersistent = isPersistent;
        IsAutoStarting = isAutoStarting;
        TargetForceBody = forceBody;
    }

    #region Add

    public void Add(params Force[] forces)
    {
        AddForces(forces, forces.Length);
    }
    
    public void Add(List<Force> forces)
    {
        AddForces(forces, forces.Count);
    }

    private void AddForces(IList<Force> targetCollection, int collectionLength)
    {
        if (collectionLength <= 0) return;
        for (int i = 0; i < collectionLength; i++)
        {
            _forceQueue.Add(targetCollection[i]);
        }
        if(IsAutoStarting)Start();
    }

    #endregion

    
    public void Start()
    {
        if(_isStarted) return;
        _isStarted = true;
        Next();
    }
    
    private void Next()
    {
        var forceCount = _forceQueue.Count;
        if(forceCount <= 0 )
        {
            _isStarted = false;
            OnFinish?.Invoke();
            return;
        }
        var force = _forceQueue[0];
        _forceQueue.RemoveAt(0);

        OnNext?.Invoke(force);
        TargetForceBody.Add(force, new CallbackConfig()
        {
            OnFinish = Next,
            OnCancel = Cancel
        });
        
    }
    
    public void Cancel()
    {
        if(!IsPersistent)
        {
            _forceQueue.Clear();
            OnCancel?.Invoke();
            _isStarted = false;
        }
        else Next();
    }
}
