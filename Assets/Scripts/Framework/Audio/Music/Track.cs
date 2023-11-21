using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable] 
public class Track
{
    [SerializeField] private string trackName;
    public bool IsPlaying { get; set; }
    [Serializable] public struct TrackLayer
    {
        [field: SerializeField] public bool Disabled { get; private set; }
        [field: SerializeField] public AudioClip Clip { get; private set; }
    }
    [field: SerializeField] public TrackLayer[] layers { get; private set; }
}
