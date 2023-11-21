using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTransposer transposer;
    [SerializeField] private float easeTime;
    
    private void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body).GetComponent<CinemachineTransposer>();
    }

    
    public void UpdateZoom(float zoom)
    {
        transposer.m_FollowOffset.z += (zoom - transposer.m_FollowOffset.z) * easeTime; 
    }

}
