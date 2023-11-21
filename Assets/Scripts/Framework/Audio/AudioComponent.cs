using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class AudioComponent : MonoBehaviour
{
    [SerializeField] private AudioData[] audioDatas;
    [SerializeField] private int crossfadeTime = 100;

    private void Update()
    {
        foreach (var audioData in audioDatas)
        {
            if (audioData.cooldownTimer <= 0) continue;
            
            audioData.cooldownTimer -= Time.deltaTime;
        }
    }

    public void PlaySound(int id)
    {
        if (id >= audioDatas.Length) return;
        PlaySound(audioDatas[id]);
    }

    public void PlaySound(string soundName)
    {
        AudioData audioData = audioDatas.FirstOrDefault(data => data.name == soundName);
        if (audioData != null) PlaySound(audioData);
    }

    private void PlaySound(AudioData audioData)
    {
        if (audioData.audioClips.Length < 1) return;
        
        AudioClip clipToPlay = audioData.audioClips[audioData.GetCurrentAudioClipIndex()];
        
        if (audioData.cooldownTimer > 0) return;

        ResetSource(audioData);
        
        if (audioData.isLooping) audioData.source.Play();
        else audioData.source.PlayOneShot(clipToPlay);
        
        audioData.cooldownTimer = clipToPlay.length;
    }

    private void ResetSource(AudioData targetAudioData)
    {
        targetAudioData.source.volume = 1;
    }

    public void ChangeSound(string soundName, string previousName)
    {
        AudioData previousData = audioDatas.FirstOrDefault(data => data.name == previousName);
        AudioData audioData = audioDatas.FirstOrDefault(data => data.name == soundName);
        if (previousData != null) StartCoroutine("StopSoundLoop", previousData);
        if (audioData != null) StartCoroutine("ChangeSoundEnum", audioData);
    }

    private IEnumerator ChangeSoundEnum(AudioData audioData)
    {
        audioData.source.clip = audioData.audioClips[audioData.GetCurrentAudioClipIndex()];
        audioData.source.volume = 0;
        audioData.source.loop = true;
        audioData.source.Play();
        float crossfadeDelay = 1 / (float)crossfadeTime;
        for (int i = 0; i < crossfadeTime; i++)
        {
            audioData.source.volume += crossfadeDelay;
            yield return new WaitForSeconds(crossfadeDelay);
        }
        audioData.source.volume = 1;
    }

    private IEnumerator StopSoundLoop(AudioData audioData)
    {
        float crossfadeDelay = 1 / (float)crossfadeTime;
        for (int i = crossfadeTime; i > 0; i--)
        {
            audioData.source.volume -= crossfadeDelay;
            yield return new WaitForSeconds(crossfadeDelay);
        }

        StopSource(audioData);
    }

    public void StopSound(int soundId)
    {
        StopSource(audioDatas[soundId]);
    }

    public void StopAll()
    {
        foreach (var audioData in audioDatas)
        {
            StopSource(audioData);
        }
    }

    private void StopSource(AudioData currentAudioData)
    {
        currentAudioData.source.volume = 0;
        currentAudioData.source.loop = false;
        currentAudioData.cooldownTimer = 0;
        currentAudioData.source.Stop();
    }

    protected void InstantiateAudioSources()
    {
        foreach (var audioData in audioDatas)
        {
            if (audioData.audioClips.Length < 1) return;
            
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = audioData.isLooping;
            source.volume = audioData.volume;

            audioData.source = source;
            source.clip = audioData.audioClips[0];
        }
    }

}
