using System.Linq;
using UnityEngine;

public class AudioManager : AudioComponent
{
    private static AudioManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        
        DontDestroyOnLoad(this.gameObject);
        InstantiateAudioSources();
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeAudioManagerOnLoad()
    {
        if (_instance != null) return;

        GameObject[] objects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        if (objects == null) return;

        GameObject audioManagerPrefab = objects.FirstOrDefault(prefab => prefab.GetComponent<AudioManager>() != null);
        if (audioManagerPrefab == null) return;

        GameObject.Instantiate(audioManagerPrefab);
        
        Debug.Log("AudioManager found and auto initialized");
    }
}
