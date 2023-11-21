using Extensions;
using UnityEngine;

public class EffectsMediator : MonoBehaviour
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }
    
    public void RequestParticle(string type)
    {
        ParticleManager.Instance.PlayParticle(type, _transform);
    }

    public void RequestParticleDestruction(int particleID)
    {
        ParticleManager.Instance.DestroyParticle(particleID, _transform);
    }

    public void RequestViaID(int particleID)
    {
        ParticleManager.Instance.PlayParticle(particleID, _transform);
    }
}
