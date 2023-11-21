using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

public class ForceMediator : MonoBehaviour
{
    private ForceBody _forceBody;

    private void Awake()
    {
        _forceBody = GetComponent<ForceBody>();
    }

    public void AddForce(string forceName)
    {
        var impactSettings = ForceLibrary.GetSettings(forceName);
        if (impactSettings.impact == null) return;
        var targetForce = impactSettings.impact.Clone();
        
        var direction = targetForce.Direction;
        direction.x *= _forceBody.gameObject.GetDirection();
        targetForce.Direction = direction;

        _forceBody.Add(targetForce);
    }
}