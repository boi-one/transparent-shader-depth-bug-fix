using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushableButton : Pushable
{
    [SerializeField] private UnityEvent onActivateButton = new UnityEvent();
    public void ActivateButton()
    {
       onActivateButton?.Invoke();
    }

    public override void Impact()
    {
        base.Impact();
        ActivateButton();
    }
}
