using System;
using Extensions;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class OneWayPlatform : MonoBehaviour
{
    private Ferr2DT_PathTerrain _ferrTerrain;

    [HideInInspector] public bool setOneWay = true;

    [Range(1, 180)] public float accessAngle = 180;

    [SerializeField] private bool hasGizmos = true;

    [Header("Visuals")] 
    [HideInInspector] public bool showVisualsGUI;
    [HideInInspector] public Color collisionLineColor = Color.red;
    [HideInInspector] public float collisionLineHeight = .1f;

    [Space] 
    [HideInInspector] public Color angleColor = Color.yellow;
    [HideInInspector] public float angleSize = 5f;

    [Space] 
    [HideInInspector] public GameObject visualsObject;
    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer.SetColors(Color.clear, Color.clear);
    }

    private void Update()
    {
        this.tag = setOneWay ? "OneWay" : "Untagged";
    }

    private void OnValidate()
    {
        _ferrTerrain = GetComponent<Ferr2DT_PathTerrain>();

        if (visualsObject == null) return;
        _lineRenderer = visualsObject.GetComponent<LineRenderer>();
        _lineRenderer.SetColors(Color.clear, Color.clear);
    }
    [System.Obsolete]
    private void OnDrawGizmos()
    {
        _lineRenderer.SetColors(Color.clear, Color.clear);
        if (!hasGizmos) return;
        
        _lineRenderer.SetColors(collisionLineColor, collisionLineColor);

        var points = _ferrTerrain.PathData.GetPoints(0);
        var position = transform.position.xy();
        var startPoint = points[1] + position;
        var endPoint = points[0] + position;

        // LINE
        _lineRenderer.SetWidth(collisionLineHeight, collisionLineHeight);
        _lineRenderer.SetColors(collisionLineColor, collisionLineColor);

        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);

        // ARC
        var difference = startPoint - endPoint;
        var midPoint = startPoint + difference * -0.5f;

        var normal = new Vector2(difference.y, -difference.x);

        visualsObject.transform.position = midPoint;
        visualsObject.LookAt(normal);

        var zRotation = normal.x >= 0 ? 270 : 90;
        visualsObject.transform.localEulerAngles += new Vector3(0, 0, zRotation);

        var angleRotation = visualsObject.transform.up.Rotate(accessAngle / 2);

#if UNITY_EDITOR
        Handles.color = angleColor;
        Handles.DrawSolidArc(visualsObject.transform.position, -transform.forward, angleRotation, accessAngle, angleSize);
#endif
    }
}