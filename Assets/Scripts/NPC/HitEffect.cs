using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private string hitEffectParticle = "Hit_Effect";
    [SerializeField] private string hitFlashParticle = "Hit_Flash";
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Execute(CollisionForceEvent collisionForceEvent)
    {
        if (collisionForceEvent.type != CollisionEventTypes.Knockback) return;
        
        var playerTransform = collisionForceEvent.collisionOrigin;
        var rotationDirection = gameObject.transform.position - playerTransform.position;
        
        var targetVector = playerTransform.gameObject.GetDirection() != rotationDirection.GetDirection()
            ? collisionForceEvent.newDirection
            : rotationDirection;

        var rotation = Quaternion.FromToRotation(Vector3.right, targetVector.normalized);
        ParticleManager.Instance.PlayParticle(hitEffectParticle, rotation, _transform);
        var randomInt = Random.Range(0, 180);
        rotation = Quaternion.Euler(0.0f, 0.0f, randomInt);
        ParticleManager.Instance.PlayParticle(hitFlashParticle, rotation, _transform);
    }
}
