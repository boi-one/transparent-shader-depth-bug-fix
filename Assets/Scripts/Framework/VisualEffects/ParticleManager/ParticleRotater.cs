using System;
using UnityEngine;

public class ParticleRotater : MonoBehaviour
{
    [SerializeField] private string bounceSparkParticle = "Bounce_Spark";
    [SerializeField] private string bounceCloudParticle = "Bounce_Cloud";
    
    private int _groundLayer = 6;

    public void Execute(CollisionForceEvent collisionForceEvent)
    {
        if (collisionForceEvent.type != CollisionEventTypes.Bounce) return;
        ParticleManager.Instance.PlayParticle(bounceSparkParticle, Quaternion.FromToRotation(Vector3.forward, collisionForceEvent.hit.normal), gameObject.transform);
        
        if (collisionForceEvent.hit.collider.gameObject.layer != _groundLayer) return;
        ParticleManager.Instance.PlayParticle(bounceCloudParticle, Quaternion.FromToRotation(Vector3.up, collisionForceEvent.hit.normal), gameObject.transform);
    }
}
