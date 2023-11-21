using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class HierarchySelector : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        if (target == null)
            target = gameObject;
        
        Selection.activeGameObject = target;
    }
}
#endif
