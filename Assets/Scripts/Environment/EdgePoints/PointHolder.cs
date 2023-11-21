using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointHolder {
    
    [SerializeReference] public List<Point> points;
    
    /// <summary>
    /// empty constructor
    /// </summary>
    /// <param name="aPoint"></param>
    public PointHolder()
    {
        points = new List<Point>();
    }
    
    /// <summary>
    /// constructor
    /// </summary>
    public PointHolder(Point aPoint)
    {
        points = new List<Point> { aPoint };
    }

    /// <summary>
    /// returns vector2 of the given index out of all points
    /// </summary>
    /// <param name="i"></param>
    public Point this[int i] {
        get => points[i];
        set => points[i] = value;
    }

    /// <summary>
    /// the amount of points there are.
    /// </summary>
    public int PointLenght => points.Count;
    

    /// <summary>
    /// adds new segment with anchor and control points 
    /// </summary>
    /// <param name="aPoint"></param>
    public void AddPoint(Point aPoint)
    {
        points.Add(aPoint);
    }
    
    /// <summary>
    /// removes the segment from the list
    /// </summary>
    public void RemovePoint(int index)
    {
        if (PointLenght <= 1) return;
        points.RemoveAt(index);
    }

    /// <summary>
    /// moves the given point and its anchor / control points if needed.
    /// </summary>
    /// <param name="i">the point we want to move.</param>
    /// <param name="pos">pos is the new position we want to move to.</param>
    public void MovePoint(int i, Vector2 pos)
    {
        points[i].Position = (pos);
    }

    /// <summary>
    /// get the list filled with all points
    /// </summary>
    /// <returns></returns>
    public List<Point> GetList()
    {
        return points;
    }
}

