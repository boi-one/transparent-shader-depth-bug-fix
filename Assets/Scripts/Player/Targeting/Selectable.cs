using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    public UnityEvent onSelect = new UnityEvent();
    public UnityEvent onDeselect = new UnityEvent();

    [SerializeField] private GameObject targetGameObject;

    private List<TargetSelector> _currentSelectors = new List<TargetSelector>();

    public GameObject TargetGameObject
    {
        get => targetGameObject;
        set => targetGameObject = value;
    }

    void Start()
    {
        if (TargetGameObject == null)
            TargetGameObject = gameObject;
    }

    public void Select()
    {
        onSelect?.Invoke();
    }

    public void DeSelect()
    {
        onDeselect?.Invoke();
    }

    public Vector3 GetPosition()
    {
        var position = transform.position;
        position.z = 0;
        return position;
    }

    public float GetDistance(Vector2 originPosition)
    {
        return Vector2.Distance(originPosition, GetPosition());
    }
    
    private void OnDestroy()
    {
        ResetSelectors();
    }

    public void AddSelector(TargetSelector selector)
    {
        if (_currentSelectors.Contains(selector)) return;
        _currentSelectors.Add(selector);
    }
    private void ResetSelectors()
    {
        var currentSelectorsLength = _currentSelectors.Count;
        for (int i = 0; i < currentSelectorsLength; i++)
        {
            var selector = _currentSelectors.ElementAt(i);
            if (selector == null) return;
            selector.Reset();
        }
        
        _currentSelectors.Clear();
    }
}
