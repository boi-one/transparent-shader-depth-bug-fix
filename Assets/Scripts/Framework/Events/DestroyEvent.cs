using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyEvent : MonoBehaviour
{
    public UnityEvent onDestroyed = new UnityEvent();
    private void OnDestroy()
    {
        onDestroyed?.Invoke();   
    }
}
