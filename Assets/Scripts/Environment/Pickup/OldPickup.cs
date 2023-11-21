using UnityEngine;
using UnityEngine.Events;

public class OldPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent onPickup = new UnityEvent();

    [SerializeField] private bool destroyOnPickup;
    [SerializeField] private float destroyAfterSeconds = 1;

    public void OnRangeEnter(GameObject other)
    {
        onPickup?.Invoke();
        if (!destroyOnPickup) return;
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void OnRangeExit(GameObject other)
    {
    }
    
    public void OnRangeStay(GameObject other)
    {
    }
}
