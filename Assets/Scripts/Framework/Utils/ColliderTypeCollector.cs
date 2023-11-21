using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTypeCollector<T> : MonoBehaviour
{
    public List<T> list = new List<T>();
    public UnityEvent onClassAdded = new UnityEvent();
    public UnityEvent onClassRemoved = new UnityEvent();
    private void OnTriggerEnter2D(Collider2D other)
    {
        T type = other.gameObject.GetComponent<T>();
        if (type == null) return;
        
        list.Add(type);
        onClassAdded?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        T type = other.gameObject.GetComponent<T>();
        if (type == null) return;

        list.Remove(type);
        onClassRemoved?.Invoke();
    }
}




