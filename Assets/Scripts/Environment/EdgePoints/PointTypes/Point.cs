using TMPro;
using UnityEngine;

[System.Serializable]
public class Point
{
    [SerializeReference] private string name = "point";
    [SerializeReference] private Vector3 localPosition;
    [SerializeReference] private float radius = 1f;
    [SerializeReference] private float diameter = .25f;
    [SerializeReference] private GameObject gameObject;

    public Point(Vector3 aPosition)
    {
        Position = aPosition;
    }

    public Point(Vector3 aPosition, GameObject aGameObject)
    {
        gameObject = aGameObject;
        localPosition = aPosition - aGameObject.transform.position;
    }

    public GameObject GameObject
    {
        get => gameObject;
        set
        {
            if (gameObject == null && value != null) localPosition -= value.transform.position; 
            gameObject = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            if (gameObject == null) return localPosition;
            return localPosition + gameObject.transform.position;
        }
        set
        {
            if (gameObject != null)
            {
                localPosition = value - gameObject.transform.position;
                return;
            }

            localPosition = value;
        }
    }

    public Vector3 LocalPosition
    {
        get => localPosition;
        set => localPosition = value;
    }

    public float Radius
    {
        get => radius;
        set => radius = value;
    }

    public float Diameter
    {
        get => diameter;
        set => diameter = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }
}
