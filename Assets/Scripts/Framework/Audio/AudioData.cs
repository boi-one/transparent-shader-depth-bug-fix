using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string name;
    public AudioClip[] audioClips;
    [Range(0.0f, 1.0f)] public float volume = 1;
    public bool randomOrder;
    public float cooldownTimer;
    public bool isLooping;
    
    private int _currentAudioClipIndex = 0;

    [HideInInspector] 
    public AudioSource source;

    public int GetCurrentAudioClipIndex()
    {
        if (randomOrder) 
            SelectRandomAudioClip();
        else 
            SelectNextAudioClip();
        
        return _currentAudioClipIndex;
    }

    private void SelectNextAudioClip()
    {
        _currentAudioClipIndex += 1;
        if (_currentAudioClipIndex >= audioClips.Length) _currentAudioClipIndex = 0;
    }

    private void SelectRandomAudioClip()
    {
        _currentAudioClipIndex = Random.Range(0, audioClips.Length);
    }
    
}
