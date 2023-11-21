#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor.Events;
using UnityEngine.Events;

public static class UnityEventBaseExtensions
{
    public static void RemovePersistentListener(this UnityEventBase eventBase, object targetObj, MethodInfo methodInfo)
    {
        var args = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
        if (UnityEventBase.GetValidMethodInfo(methodInfo.DeclaringType, methodInfo.Name, args) is null) return;
        
        var l = eventBase.GetPersistentEventCount();
        int idx;
        for (idx = 0; idx < l; idx++)
        {
            var currentEventTarget = eventBase.GetPersistentTarget(idx);
            if (targetObj != currentEventTarget)
                continue;
        
            var currentEventName = eventBase.GetPersistentMethodName(idx);
            if (currentEventName != methodInfo.Name) continue;
            
            UnityEventTools.RemovePersistentListener(eventBase, idx);
        }
    }
}
#endif