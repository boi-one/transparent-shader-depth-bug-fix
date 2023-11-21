using System;
using UnityEngine;

namespace Audio
{
    public class MusicHandler : MonoBehaviour
    {
        private AudioManager _audioManager;

        private string _previousMusicState = null;

        private void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            if (_audioManager == null)
            {
                Debug.LogError("AudioManager does not exist");
                enabled = false;
            } 
        }

        public void ChangeAudio(string soundName) 
        {
            if (soundName == _previousMusicState) return;
            
            _audioManager.ChangeSound(soundName, _previousMusicState);
            _previousMusicState = soundName;
        }
    }
}
