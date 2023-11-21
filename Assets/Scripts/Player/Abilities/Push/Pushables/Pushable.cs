using UnityEngine;
using UnityEngine.Events;

public class Pushable : MonoBehaviour
{
    [SerializeField] private UnityEvent OnImpact = new UnityEvent();
    
    public virtual void Impact()
    {
        OnImpact?.Invoke();
    }

    public UnityEvent GetOnImpactEvent()
    {
        return OnImpact;
    }
}
