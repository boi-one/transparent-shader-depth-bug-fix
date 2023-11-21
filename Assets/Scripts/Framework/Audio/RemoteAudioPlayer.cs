using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAudioPlayer : MonoBehaviour
{
    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();

        if (_audioManager == null)
        {
            Debug.LogError("AudioManager does not exist");
            this.enabled = false;
        }
    }

    public void PlaySound(int soundId)
    {
        if (!this.isActiveAndEnabled) return;
        _audioManager.PlaySound(soundId);
    }

    public void PlaySound(string soundName)
    {
        if (!this.isActiveAndEnabled) return;
        _audioManager.PlaySound(soundName);
    }

}
