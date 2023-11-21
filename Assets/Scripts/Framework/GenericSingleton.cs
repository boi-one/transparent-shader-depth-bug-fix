using UnityEngine;
using System.Collections;

public class GenericSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = GameObject.FindObjectOfType<T>();
            if (_instance != null) return _instance;

            GameObject container = new GameObject(typeof(T).Name);
            _instance = container.AddComponent<T>();

            return _instance;
        }
    }
}
