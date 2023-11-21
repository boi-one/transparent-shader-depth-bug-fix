#if UNITY_EDITOR
using UnityEngine.Events;

public static class UnityEventExtensions
{
    public static void RemovePersistentListener<T0>(this UnityEvent<T0> unityEvent, UnityAction<T0> action)
    {
        unityEvent.RemovePersistentListener(action.Target, action.Method);
    }

    public static void RemovePersistentListener<T0, T1>(this UnityEvent<T0, T1> unityEvent, UnityAction<T0, T1> action)
    {
       unityEvent.RemovePersistentListener(action.Target, action.Method); 
    }

    public static void RemovePersistentListener<T0, T1, T2>(this UnityEvent<T0, T1, T2> unityEvent, UnityAction<T0, T1, T2> action)
    {
        unityEvent.RemovePersistentListener(action.Target, action.Method);
    }

    public static void RemovePersistentListener<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> action)
    {
        unityEvent.RemovePersistentListener(action.Target, action.Method);
    }
}
#endif