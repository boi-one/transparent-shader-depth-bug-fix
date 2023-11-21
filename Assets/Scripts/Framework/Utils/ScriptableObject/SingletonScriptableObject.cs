using System.Linq;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                var instances = Resources.FindObjectsOfTypeAll<T>();
                if (instances.Length == 0)
                {
                    Debug.LogWarning("No scriptable object asset found");
                    return null;
                }

                _instance = instances[0];
            }
            
            return _instance;
        }
    }
}
