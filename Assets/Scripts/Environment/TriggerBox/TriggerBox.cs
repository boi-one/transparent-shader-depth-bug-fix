using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour, IInteractable
{
    [SerializeField] private List<string> requiredTags = new List<string>();
    [SerializeField] private UnityEvent onBoxEnter = new UnityEvent();
    [SerializeField] private UnityEvent onBoxExit = new UnityEvent();
    [SerializeField] private UnityEvent onBoxStay = new UnityEvent();

    private void Start()
    {
        Destroy(GetComponent<MeshRenderer>());
    }

    public virtual void OnRangeEnter(GameObject other)
    {
        if (!other.gameObject.HasTags(requiredTags)) return;
        onBoxEnter?.Invoke();
    }

    public virtual void OnRangeExit(GameObject other)
    {
        if (!other.gameObject.HasTags(requiredTags)) return;
        onBoxExit?.Invoke();
    }

    public virtual void OnRangeStay(GameObject other)
    {
        if (!other.gameObject.HasTags(requiredTags)) return;
        onBoxStay?.Invoke();
    }
}