using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CollisionEventDispatcher : MonoBehaviour
{
    [SerializeField] private LayerMask allowedLayers;
    
    public UnityEvent onEnter;
    public UnityEvent onStay;
    public UnityEvent onLeave;

    public LayerMask AllowedLayers => allowedLayers;
    
    public void OnEnter()
    {
        onEnter?.Invoke();
    }

    public void OnStay()
    {
        onStay?.Invoke();
    }

    public void OnLeave()
    {
        onLeave?.Invoke();
    }
}
