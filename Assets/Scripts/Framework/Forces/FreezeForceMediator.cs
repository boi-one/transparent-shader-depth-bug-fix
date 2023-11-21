using UnityEngine;
using UnityEngine.Events;

public class FreezeForceMediator : MonoBehaviour
{
    [SerializeField] private UnityEvent onFreeze = new UnityEvent();

    public void Freeze(string type)
    {
        FreezeForceSystem.Instance.AddFreeze(type, gameObject);
        onFreeze.Invoke();
    }
}
