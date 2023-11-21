using System.Collections.Generic;
using Extensions;
using Points.PointTypes;
using Unity.Mathematics;
using UnityEngine;

public class PointCreator : MonoBehaviour
{
    [SerializeField] private bool shouldAutoGenerate = true;
#if UNITY_EDITOR
    [Header("colours")] public Color defaultColor = Color.white;
    public Color removeColor = Color.red;
    public Color selectedColor = Color.yellow;
    public Color hoverColor = Color.yellow;
    public bool drawRadius = true;
    public bool createDraggablePoints = true;
#endif
    [SerializeReference] public PointHolder pointHolder;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float hitSizeMultiplier = 1;

#if UNITY_EDITOR
    /// <summary>
    /// Create new PointHolder en replace the old one. 
    /// </summary>
    public void CreatePath()
    {
        var position = transform.position;
        var gameObj = gameObject;
        Point point = (createDraggablePoints) ? new GrabbablePoint(position, gameObj) : new Point(position, gameObj);
        pointHolder = new PointHolder(point);
    }

    /// <summary>
    /// Create new PointHolder en replace the old one. 
    /// </summary>
    public void Reset()
    {
        var position = transform.position;
        var gameObj = gameObject;
        Point point = (createDraggablePoints) ? new GrabbablePoint(position, gameObj) : new Point(position, gameObj);

        if (pointHolder == null)
        {
            pointHolder = new PointHolder(point);
            return;
        }

        pointHolder.points = new List<Point>();
        pointHolder.AddPoint(point);
    }
#endif

    /// <summary>
    /// Checks all point position and checks there furest away and saved it
    /// to get the collider one the sides after that we check the point with the biggest radius and we add that as well.  
    /// </summary>
    /// <param name="aList"></param>
    /// <returns></returns>
    public Vector2 GetBoxColliderSize(List<Point> aList)
    {
        float totalX = 0;
        float totalY = 0;
        Vector2 center = Vector2.zero;
        float biggestDistanceXFromCenter = 0;
        float biggestDistanceYFromCenter = 0;
        float biggestRadius = 0;
        Vector2 size = Vector2.zero;
        int listCount = aList.Count;

        foreach (Point point in aList)
        {
            totalX += point.Position.x;
            totalY += point.Position.y;
        }

        center = new Vector2(totalX / listCount, totalY / listCount);
        Vector3 difference = Vector3.zero;
        if ((!float.IsNaN(center.x) || !float.IsNaN(center.y)))
            difference = new Vector3(center.x, center.y, 0) - gameObject.transform.position;
        boxCollider.offset = difference;

        foreach (Point point in aList)
        {
            if (point.Radius > biggestRadius) biggestRadius = point.Radius;
            float distanceX = math.abs(center.x - point.Position.x);
            float distanceY = math.abs(center.y - point.Position.y);

            if (distanceX > biggestDistanceXFromCenter) biggestDistanceXFromCenter = distanceX;
            if (distanceY > biggestDistanceYFromCenter) biggestDistanceYFromCenter = distanceY;
        }

        size = new Vector2(biggestDistanceXFromCenter + biggestRadius, biggestDistanceYFromCenter + biggestRadius);

        return size * 2;
    }

    /// <summary>
    /// fixes the GameObject to be centered between all points and makes the size so it goes over all points. 
    /// </summary>
    public void FixCollider()
    {
        if (boxCollider == null) boxCollider = gameObject.GetOrAddComponent<BoxCollider2D>();
        Vector2 size = GetBoxColliderSize(pointHolder.GetList());
        boxCollider.size = size * hitSizeMultiplier;
        boxCollider.isTrigger = true;
    }

    /// <summary>
    /// Add the " AutoGenerateEdgePoints " to the GameObject en generates the points. 
    /// </summary>
    public void GenerateEdgePoints()
    {
        if (!shouldAutoGenerate) return;
        AutoGenerateEdgePoints autoGenerateEdgePoints = gameObject.GetOrAddComponent<AutoGenerateEdgePoints>();
        autoGenerateEdgePoints.Generate();
    }
}