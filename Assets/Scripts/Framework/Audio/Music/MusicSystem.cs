using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSystem : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    public float Volume
    {
        get
        {
            mixer.GetFloat("MasterVolume", out float value);
            return Mathf.Pow(10, value/decibelMultiplier);
        }
        set => mixer.SetFloat("MasterVolume", Mathf.Log10(value) * decibelMultiplier);
    }
    [SerializeField] private float transitionDelay = 1f;
    [SerializeField] private int decibelMultiplier = 20;
    
    [SerializeField] private Track[] tracks;
    [SerializeField] private TrackPlayer[] trackPlayers;
    
    private TrackPlayer _currentTrackPlayer;
    private int _currentTrackId;
    private void Start()
    {
        SetCurrentTrackPlayer(0);
        TrackPlayer.SetMixer(mixer);
        SetCurrentTrack(tracks[0]);
    }
    public void Play()
    {
        if (_currentTrackPlayer.currentTrack.IsPlaying) return;
        _currentTrackPlayer.StartFadeIn();
    }
    public void Stop() => _currentTrackPlayer.StartFadeOut(); 
    public void PlayTrack(int trackIndex) 
    {
        if (_currentTrackPlayer.currentTrack == tracks[trackIndex]) return;
        StartCoroutine(Transition(trackIndex));
    }
    private IEnumerator Transition(int index)
    {
        _currentTrackPlayer.StartFadeOut();
        yield return new WaitForSeconds(transitionDelay);
        SwapTrackPlayers();
        SetCurrentTrack(tracks[index]);
        _currentTrackPlayer.StartFadeIn();
    }
    //deze FadeLayer in/out methods kunnen ook dry door in 1 method te zetten met nog een boolean als parameter
    public void FadeInLayer(params int[] index) => _currentTrackPlayer.ToggleFadeLayer((index,true));
    public void FadeOutLayer(params int[] index) => _currentTrackPlayer.ToggleFadeLayer((index,false));
    public void FadeInLayer(TrackPlayer targetPlayer, params int[] index) => targetPlayer.ToggleFadeLayer((index, true));
    public void FadeOutLayer(TrackPlayer targetPlayer, params int[] index) => targetPlayer.ToggleFadeLayer((index, true));
    private void SetCurrentTrack(Track newTrack) => _currentTrackPlayer.currentTrack = newTrack;
    private void SwapTrackPlayers() => SetCurrentTrackPlayer(trackPlayers[0] == _currentTrackPlayer ? 1 : 0);
    private void SetCurrentTrackPlayer(int index)
    {
        _currentTrackPlayer = trackPlayers[index];
        _currentTrackId = Array.IndexOf(trackPlayers, _currentTrackPlayer);
    }
} 
