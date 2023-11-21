using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadUtility : GenericSingleton<ThreadUtility>
{
    public static void StartDelayedTask(IEnumerator aEnumerator)
    {
        Instance.StartCoroutine(aEnumerator);
    }
    
    public static void StopDelayedTask(IEnumerator aEnumerator)
    {
        Instance.StopCoroutine(aEnumerator);
    }
}