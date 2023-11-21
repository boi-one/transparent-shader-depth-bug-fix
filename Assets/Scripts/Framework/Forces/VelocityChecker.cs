using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VelocityChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent onThresholdExceeded = new UnityEvent();

    private ForceBody _forceBody;

    private void Awake()
    {
        _forceBody = GetComponent<ForceBody>();
    }

    public void CheckVelocity(float threshold)
    {
        if (_forceBody.DesiredVelocity.magnitude < threshold) return;
        onThresholdExceeded?.Invoke();
    }
}
