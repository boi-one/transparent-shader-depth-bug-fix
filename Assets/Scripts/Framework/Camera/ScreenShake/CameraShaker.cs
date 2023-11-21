using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;

    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _perlinNoiseChannel;
        
    private void Awake()
    {
        _perlinNoiseChannel =_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float duration, float frequency)
    {
        if (!isEnabled) return;

        _perlinNoiseChannel.m_AmplitudeGain = intensity;
        _perlinNoiseChannel.m_FrequencyGain = frequency;

        StartCoroutine(ShakeTimer(duration));       
    }

    private IEnumerator ShakeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        _perlinNoiseChannel.m_AmplitudeGain = 0f;
        _perlinNoiseChannel.m_FrequencyGain = 0f;
    }
}
