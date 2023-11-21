using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelReloader : MonoBehaviour
{
    private bool _isReloading;
    private int staticObjectsSceneIndex = 1;
    [SerializeField] private UnityEvent onReset = new UnityEvent();

    private void Start()
    {
        staticObjectsSceneIndex = SceneManager.sceneCountInBuildSettings-1;
        
        #if !UNITY_EDITOR
        Reset();
        #endif
    }

    public void Reset()
    {
        if (_isReloading) return;
        Unload();
        _isReloading = true;
        onReset.Invoke();
    }

    private void Unload()
    {
        StartCoroutine(UnloadScene());
    }

    IEnumerator UnloadScene()
    {
        if (SceneManager.sceneCount > 1)
        {
            AsyncOperation ao = SceneManager.UnloadSceneAsync(staticObjectsSceneIndex);
            yield return ao;            
        }
        LoadScene();
        yield return new WaitForSeconds(1);
        _isReloading = false;
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(staticObjectsSceneIndex, LoadSceneMode.Additive);
    }
}
