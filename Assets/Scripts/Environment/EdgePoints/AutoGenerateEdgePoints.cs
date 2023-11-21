using System;
using System.Collections.Generic;
using Extensions;
using FerrPoly2Tri;
using Points.PointTypes;
using Skills.Grabbing;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class AutoGenerateEdgePoints : MonoBehaviour
{
    private PointCreator _creator;
    private List<Point> _pointList = new List<Point>();
    private List<Point> _edgePoints = new List<Point>();
    [SerializeField] private Vector2 offset = new Vector2(1.9f,0.7f);
    [SerializeField] private float topOrDownTreshold = .1f;
    [SerializeField] private float leftOrRightTreshold = 1f;
    [SerializeField] private float pointDistance = 3.0f;
    [SerializeField] private bool keepUpdating = false;

    private List<string> activeSelectionNames = new List<string>()
    {
        "Terrain",
        "Points"
    };

    public bool KeepUpdating
    {
        get => keepUpdating;
        set => keepUpdating = value;
    }

    private Transform _transform;

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying && Selection.activeObject && Selection.activeObject.name.ContainsAny(activeSelectionNames) )
        {
            Generate();
        }
    }
#endif
    /// <summary>
    /// Start function for generating the points by getting all ferr2d points and checks angels.
    /// </summary>
    public void Generate()
    {
        _transform = transform;

        _creator = gameObject.GetOrAddComponent<PointCreator>();

        CreatePoints();
        if (!(_pointList.Count <= 2))
        {
            CheckForEdgePoints();
        }
    }

    /// <summary>
    /// gets all the ferr2d points and create GrabablePoints from these locations.
    /// </summary>
    private void CreatePoints()
    {
        List<Vector2> startingList = ListFromFerr();
        if (startingList.IsEmpty()) return;
        List<Point> endList = new List<Point>();

        if (startingList.Count <= 2)
        {
            Debug.LogWarning("Not enough points in ferr2d need at least 3");
            return;
        }

        for (int i = 0; i < startingList.Count; i++)
        {
            Vector2 current = startingList.Get(i);

            GrabbablePoint grabbablePoint = new GrabbablePoint(current);
            grabbablePoint.Radius = pointDistance;
            endList.Add(grabbablePoint);
        }

        _pointList = endList;
    }

    private List<Vector2> ListFromFerr()
    {
        List<Vector2> newList = new List<Vector2>();
        if (gameObject.HasComponent<Ferr2DT_PathTerrain>())
        {
            newList = gameObject.GetComponentInParent<Ferr2DT_PathTerrain>().PathData.GetPoints(0);
        }
        else if (gameObject.HasComponentInParent<Ferr2DT_PathTerrain>())
        {
            newList = gameObject.GetComponentInParent<Ferr2DT_PathTerrain>().PathData.GetPoints(0);
            _transform.position = _transform.parent.transform.position;
        }
        else
        {
            Debug.LogWarning("ferr2d path not found!");
            return null;
        }

        return newList;
    }

    /// <summary>
    /// loops all the points and checks if its on an edge. if it is we add it to the list of edge points what will be our final list.
    /// that list will overwrite the current points and we will have all the edge points
    /// </summary>
    private void CheckForEdgePoints()
    {
        bool wasSteepUp = false;
        bool wasSteepDown = false;

        bool wasRight = false;
        bool wasLeft = false;

        List<Point> points = new List<Point>();
        for (var i = 0; i < _pointList.Count; i++)
        {
            Point currentPoint = _pointList.Get(i);
            Point nextPoint = _pointList.Get(i + 1);
            Point prevPoint = _pointList.Get(i - 1);
            GrabbablePoint grabbablePoint;

            Vector2 nextDirection = nextPoint.Position - currentPoint.Position;
            Vector2 previousDirection = prevPoint.Position - currentPoint.Position;

            var isSteepUp = nextDirection.IsSameDirectionAs(Vector2.up, topOrDownTreshold);
            var isSteepDown = nextDirection.IsOppositeDirectionAs(Vector2.up, topOrDownTreshold);

            var isLeftEdge = false;
            var isRightEdge = false;

            var checkLeft = !isSteepDown && !isSteepUp && wasSteepUp;
            var isEdgePoint = isSteepDown && !wasSteepDown && !wasSteepUp || !isSteepDown && !isSteepUp && wasSteepUp;

            if (isEdgePoint)
            {
                if (checkLeft) isLeftEdge = nextDirection.IsSameDirectionAs(Vector2.left, leftOrRightTreshold);
                else isRightEdge = true;

                grabbablePoint = new GrabbablePoint(currentPoint.Position.AddVector(_transform.position));

                Vector2 grabablePointPosition = grabbablePoint.Position;
                if (isLeftEdge)
                {
                    grabbablePoint.GrabType = GrabType.RIGHTEDGE;
                    grabablePointPosition.x += offset.x;
                }
                else if (isRightEdge)
                {
                    grabbablePoint.GrabType = GrabType.LEFTEDGE;
                    grabablePointPosition.x -= offset.x;
                }
                grabablePointPosition.y += offset.y;

                grabbablePoint.Position = grabablePointPosition;
                grabbablePoint.Radius = pointDistance;
                grabbablePoint.Name = i.ToString();
                grabbablePoint.GameObject = this.gameObject;
                points.Add(grabbablePoint);
            }

            wasSteepUp = isSteepUp;
            wasSteepDown = isSteepDown;
            wasRight = isRightEdge;
            wasLeft = isLeftEdge;
        }

        _edgePoints = points;
        if (_creator == null || _creator.pointHolder == null || _creator.pointHolder.points == null) return;
        _creator.pointHolder.points = (points);
    }
}
