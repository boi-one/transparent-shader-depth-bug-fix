
using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraZoom))]

public class CameraController : MonoBehaviour
{
    [Header("zoomData")]
    [SerializeField] private CinemachineTargetGroup playerTargets;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float defaultOffset;
    [SerializeField] private Vector2 zoomMultiplier;
    [SerializeField] private string interestPointTag;

    private Transform[] _furthestTargetTransforms = new Transform[4];

    private List<Collider2D> _interestingPointCollider = new List<Collider2D>();
    private float _targetCameraZoom = 0.0f;

    private const float TargetPointWeight = 0.5f;

    private void Start()
    {
        var currenBuildIndex = SceneManager.GetSceneByBuildIndex(0);
        foreach (var g in _furthestTargetTransforms)
        {
            g.name = "InterestingPoint";
            SceneManager.MoveGameObjectToScene(g.gameObject, currenBuildIndex);
        }

    }

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            _furthestTargetTransforms[i] = new GameObject().transform;
        }
    }

    private void LateUpdate()
    {
        if (playerTargets.IsEmpty) return;

        _targetCameraZoom = GetZoom();
        _targetCameraZoom += defaultOffset;
        _targetCameraZoom = Mathf.Clamp(_targetCameraZoom, minZoom, maxZoom);
        _targetCameraZoom = -Mathf.Abs(_targetCameraZoom);

        GetComponent<CameraZoom>().UpdateZoom(_targetCameraZoom);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(interestPointTag))
        {
            return;
        }
        _interestingPointCollider.Add(other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(interestPointTag))
        {
            return;
        }

        RemoveInterestingPointTarget();
        _interestingPointCollider.Remove(other);
    }

    private float GetZoom()
    {
        if (_interestingPointCollider.Count == 0) return GetPlayerOnlyZoom();

        Vector3 center = GetInterestingPointsCenter();

        List<Vector3> corners = new List<Vector3>();
        Vector3[] colliderPoints = _interestingPointCollider[0].GetCornerPoints();

        corners.AddRange(colliderPoints);
        corners.AddRange(GetTargetPositions());

        Vector3 furthestPoint = center.GetFurthest(corners);

        if (Vector3.Distance(center, furthestPoint) < maxZoom)
        {
            UpdateTargetPoints(colliderPoints);
            return GetMultipliedDistance(center, furthestPoint);
        }

        RemoveInterestingPointTarget();
        return GetPlayerOnlyZoom();
    }

    private float GetPlayerOnlyZoom()
    {
        RemoveInterestingPointTarget();

        Vector3 center = GetDefaultCenter();

        List<Vector3> points = new List<Vector3>(GetTargetPositions());

        Vector3 furthest = center.GetFurthest(points);

        return GetMultipliedDistance(furthest, center);
    }


    private Vector3 GetDefaultCenter()
    {
        Vector3 center = new Vector3(0, 0, 0);
        foreach (var playerTargetsMTarget in playerTargets.m_Targets)
            center += playerTargetsMTarget.target.position;

        center /= playerTargets.m_Targets.Length;

        return center;
    }

    private Vector3 GetInterestingPointsCenter()
    {
        var center = new Vector3(0, 0, 0);
        foreach (var playerTargetsMTarget in playerTargets.m_Targets)
            center += playerTargetsMTarget.target.position;
        foreach (var box2DCornerPoint in _interestingPointCollider[0].GetCornerPoints())
            center += box2DCornerPoint;

        center /= playerTargets.m_Targets.Length + _interestingPointCollider[0].GetCornerPoints().Length;

        return center;
    }


    private void AddInterestingPointTarget()
    {
        var targets = new List<CinemachineTargetGroup.Target>(playerTargets.m_Targets);

        if (targets.Count(target => _furthestTargetTransforms.Contains(target.target)) > 0) return;

        for (var i = _furthestTargetTransforms.Length - 1; i >= 0; i--)
        {
            var pointTarget = new CinemachineTargetGroup.Target
            {
                radius = 1,
                weight = TargetPointWeight,
                target = _furthestTargetTransforms[i]
            };

            targets.Add(pointTarget);
        }
        playerTargets.m_Targets = targets.ToArray();
    }
    private void RemoveInterestingPointTarget()
    {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>(playerTargets.m_Targets);

        if (targets.Count(target => _furthestTargetTransforms.Contains(target.target)) == 0) return;


        for (var i = targets.Count - 1; i >= 0; i--)
        {
            if (_furthestTargetTransforms.Contains(targets[i].target)) targets.RemoveAt(i);
        }

        playerTargets.m_Targets = targets.ToArray();
    }

    private void UpdateTargetPoints(Vector3[] colliderPoints)
    {
        for (var i = colliderPoints.Length - 1; i >= 0; i--) _furthestTargetTransforms[i].position = colliderPoints[i];
        AddInterestingPointTarget();
    }

    private float GetMultipliedDistance(Vector3 from, Vector3 to)
    {
        var xDiff = (from.x - to.x) * zoomMultiplier.x;
        var yDiff = (from.y - to.y) * zoomMultiplier.y;
        return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    private Vector3[] GetTargetPositions()
    {
        Vector3[] positions = new Vector3[playerTargets.m_Targets.Length];
        for (var i = playerTargets.m_Targets.Length - 1; i >= 0; i--)
        {
            positions[i] = playerTargets.m_Targets[i].target.position;
        }

        return positions;
    }
}
