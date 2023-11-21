using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetSelector : TargetSelector
{
    private Camera _mainCamera;

    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main;
    }

    protected override bool IsSelectable(Selectable selectable)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
        var selectableInView = GeometryUtility.TestPlanesAABB(planes, _spottedSelectables[selectable].bounds);
        return selectableInView;
    }
}
