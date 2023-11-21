using UnityEngine;

public class CollisionForceEvent : ForceEvent
{
    public Vector3 velocity;
    public Vector3 newDirection;
    public Transform collisionOrigin;
    public ForceBody targetForcebody;
    public RaycastHit hit;
    public bool isGroundHit;
    public bool resetsGravity;
}
