using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAudioPlayer : AudioComponent
{
    private void Awake() => InstantiateAudioSources();
}
