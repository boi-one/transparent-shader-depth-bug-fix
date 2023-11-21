using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TrackPlayer : MonoBehaviour
{
    [HideInInspector] public Track currentTrack;
    private static AudioMixer Mixer { get; set; }
    [SerializeField] private float minVolume = 0.0001f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private int decibelMultiplier = 20;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeMultiplier = 10f;
    [SerializeField] private int layerAmount = 5;

    private bool _isFading;
    private string _songName;
    private AudioMixerGroup _parentMixerGroup;
    private string[] _layerName;
    private AudioMixerGroup[] _assignedMixerGroups;
    private List<AudioSource> _trackSources; //kan ook [HideInInspector] hebben

    private void Start()
    {
        _layerName = new string[layerAmount];
        _assignedMixerGroups = new AudioMixerGroup[layerAmount];
        _trackSources = new List<AudioSource>(layerAmount);
    }
    [Serializable] public struct MixerGroup
    {
        public AudioMixerGroup group;
        public string exposedParam;
    }
    [SerializeField] private MixerGroup[] mixerGroups;
    public static void SetMixer(AudioMixer targetMixer) => Mixer = targetMixer;
    private void MuteLayers()
    {
        var l = currentTrack.layers.Length;
        for (var i = 0; i < l; i++)
        {
            if (!currentTrack.layers[i].Disabled) continue;
            SetVolume(i, minVolume);
        }
    }
    private void AssignMixerGroups()
    {
        _parentMixerGroup = mixerGroups[0].group;
        _songName = mixerGroups[0].exposedParam;
        var l = currentTrack.layers.Length; 
        for (int i = 0; i < l; i++)
        {
            _assignedMixerGroups[i] = mixerGroups[i+1].group;
            _layerName[i] = mixerGroups[i+1].exposedParam;
        }
    }
    private void CreateSources()
    {
        var l = currentTrack.layers.Length;
        for (int i = 0; i < l; i++)
        {
            var newSource = gameObject.AddComponent<AudioSource>();
            _trackSources.Add(newSource);
        }
    }
    private void InitializeSources()
    {
        var l = _trackSources.Count;
        for (int i = 0; i < l; i++)
        {
            var source = _trackSources[i];
            source.clip = currentTrack.layers[i].Clip; // zet clip van audiosource
            source.loop = true;
            source.outputAudioMixerGroup = _assignedMixerGroups[i]; // set audiosource output naar de audiomixergroup
        }
    }
    private void ClearSources()
    {
        var l = _trackSources.Count;
        for (int i = 0; i < l; i++)
        {
            Destroy(_trackSources[i]);
        }
        _trackSources.Clear();
    }
    private void SetVolume(int index, float value) => Mixer.SetFloat(_layerName[index], Mathf.Log10(value) * decibelMultiplier);
    public void ToggleFadeLayer(params (int[] indexes, bool fadeToggle)[] targetValues)
    {
        if (_isFading) return;
        var targetValuesLength = targetValues.Length;
        for (var i = 0; i < targetValuesLength; i++)
        {
            var indexesLength = targetValues[i].indexes.Length;
            for (int j = 0; j < indexesLength; j++)
            {
                if (_layerName[targetValues[i].indexes[j]] == null || _layerName == null) return;
                StartCoroutine(Fade((_layerName[targetValues[i].indexes[j]], targetValues[i].fadeToggle)));
            }
        }
    }
    public void StartFadeIn() => StartCoroutine(FadeIn());
    public void StartFadeOut() => StartCoroutine(FadeOut());
    private IEnumerator FadeOut()
    {
        if (_songName != null) StartCoroutine(Fade((_songName, false)));
        currentTrack.IsPlaying = false;
        yield return new WaitForSeconds(fadeDuration);
        if (_trackSources == null || _trackSources.IsEmpty()) yield return null;
        var l = _trackSources.Count;
        for (var i = 0; i < l; i++)
        {
            _trackSources[i].Stop();
        }
        ClearSources();
        
    }
    private IEnumerator FadeIn()
    {
        if (_trackSources == null || _trackSources.IsEmpty())
        {
            CreateSources();
            AssignMixerGroups();
            InitializeSources();
            MuteLayers();
        }
        var l = _trackSources.Count;
        for (int i = 0; i < l; i++)
        {
            _trackSources[i].Play();
        }
        currentTrack.IsPlaying = true;
        StartCoroutine(Fade((_songName, true)));
        yield return new WaitForSeconds(fadeDuration);
    }
    private IEnumerator Fade(params (string exposedParam,bool fadeToggle)[] targetValues)
    {
        _isFading = true;
        var currentVolumes = new float[targetValues.Length];
        var l = targetValues.Length;
        for (var i = 0; i < l; i++)
        {
            Mixer.GetFloat(targetValues[i].exposedParam, out currentVolumes[i]);
            currentVolumes[i] = Mathf.Pow(fadeMultiplier, currentVolumes[i] / decibelMultiplier);
            StartCoroutine(FadeLayer(targetValues[i].exposedParam, currentVolumes[i], targetValues[i].fadeToggle));
        }
        yield return null;
        _isFading = false;
    }

    private IEnumerator FadeLayer(string exposedParam, float currentVolume, bool fadeIn)
    {
        var currentTime = 0f;
        if (currentVolume == minVolume) yield return null; 
        var targetValue = fadeIn ? maxVolume : minVolume;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            var newVol = Mathf.Lerp(currentVolume, targetValue, currentTime / fadeDuration);
            Mixer.SetFloat(exposedParam, Mathf.Log10(newVol) * decibelMultiplier);
            yield return null;
        }
    }
}
