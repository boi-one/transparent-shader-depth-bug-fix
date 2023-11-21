using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraZOffset : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineTransposer transposer;
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [SerializeField] private float minZoom = 10;
    [SerializeField] private float maxZoom = 25;
    [SerializeField] private float defaultOffset = 0;
    [SerializeField] private float zoomMultiplier = 1.0f;
    
    private void Awake()
    {
        transposer = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body).GetComponent<CinemachineTransposer>();
    }

    private void FixedUpdate()
    {
        Zoom();
    }

    public void Zoom()
    {
        if (targetGroup.m_Targets.Length == 0) return;
        CinemachineTargetGroup.Target target = targetGroup.m_Targets.OrderBy(target => Vector2.Distance(target.target.transform.position, virtualCamera.transform.position)).LastOrDefault();
        float distance = Vector2.Distance(virtualCamera.transform.position, target.target.position);

        distance += defaultOffset;
        distance *= zoomMultiplier;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        transposer.m_FollowOffset.z = -distance;
    }
}
