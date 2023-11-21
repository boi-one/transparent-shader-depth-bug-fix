using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private Camera orthoghrapicCamera;
    [SerializeField] private float followoffset;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTransposer transposer;

    private void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body).GetComponent<CinemachineTransposer>();
    }

    private void LateUpdate()
    {
        orthoghrapicCamera.orthographicSize = transposer.m_FollowOffset.z * -followoffset;
    }
}