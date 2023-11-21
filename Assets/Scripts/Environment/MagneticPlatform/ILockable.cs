using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ILockable
{
    Transform Transform { get; } 
    ForceBody ForceBody { get; }
    Transform RotationTarget { get; set; }
    UnityEvent OnLockRelease { get; }
    void StateTrigger(string triggerName, bool value);
    
}
