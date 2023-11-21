using System.Collections;
using System.Linq;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : GenericSingleton<ParticleManager>
{
    [SerializeField] private ParticleData[] particles;
    private ParticleSystem _newParticleSystem;
    private VisualEffect _newVFX;
    private Vector3 _objectScale;
    
    private void Awake()
    {
        _objectScale = gameObject.transform.localScale;

        foreach (var particle in particles)
        {
            switch (particle.offsetPositions)
            {
                case OffsetPositions.Top:
                    particle.offset = new Vector3(0, _objectScale.y / 2, 0);
                    break;
                case OffsetPositions.Left:
                    particle.offset = new Vector3(-_objectScale.x / 2, 0, 0);
                    break;
                case OffsetPositions.Right:
                    particle.offset = new Vector3(_objectScale.x / 2, 0, 0);
                    break;
                case OffsetPositions.Bottom:
                    particle.offset = new Vector3(0, -_objectScale.y / 2, 0);
                    break;
            }
        }
    }

    public IEnumerator Cooldown(ParticleData particle)
    {
        particle.canSpawn = false;
        yield return new WaitForSeconds(particle.particleFireRate);
        particle.canSpawn = true;
    }

    public void PlayParticle(string type, Transform originTransform)
    {
        PlayParticleType(type, Quaternion.identity, originTransform);
    }

    public void PlayParticle(string type, Quaternion rotation, Transform originTransform)
    {
        PlayParticleType(type, rotation, originTransform);
    }

    public void PlayParticle(int id, Transform originTransform)
    {
        PlayParticleId(id, Quaternion.identity, originTransform);
    }

    public void PlayParticle(int id, Quaternion rotation, Transform originTransform)
    {
        PlayParticleId(id, rotation, originTransform);
    }

    private void PlayParticleType(string type, Quaternion rotation, Transform originTransform)
    {
        if (!particles.Any(data => data.name == type)) return;
                
        PlayParticle(particles.First(data => data.name == type), rotation, originTransform);
    }

    private void PlayParticleId(int id, Quaternion rotation, Transform originTransform)
    {
        if (id >= particles.Length) return;
        PlayParticle(particles[id], rotation, originTransform);
        particles[id].id = id;
    }

    private void PlayParticle(ParticleData particle, Quaternion rotation, Transform originTransform)
    {
        if (!particle.isEnabled) return;
        
        StartCoroutine(CooldownTimer(particle.delayedSpawnTime, particle, rotation, originTransform));
    }

    private bool ParticleHasParent(ParticleData particle, Transform parentTransform)
    {
        var count = particle.instantiatedParticles.Count;
        for (var i = 0; i < count; i++)
        {
            var currentInstantiatedParticle = particle.instantiatedParticles[i];
            if (currentInstantiatedParticle == null) continue;
            if (currentInstantiatedParticle.transform.parent.gameObject == parentTransform.gameObject) return true;
        }

        return false;
    }

    private void SetInstantiateValues(ParticleData particle, Quaternion rotation, Transform originTransform)
    {
        var isFlipped = false;
        if (particle.isConstant && !particle.instantiatedParticles.IsEmpty() && ParticleHasParent(particle, originTransform)) return;
        
        if (!particle.canSpawn) return;

        if (particle.isAutoOrienting)
            isFlipped = originTransform.gameObject.GetDirection() == -1;
        
        if(particle.hasCoolDown) StartCoroutine(Cooldown(particle));
        if (particle.isConstant)
        {
            particle.parent = originTransform.gameObject;
        }
        
        GameObject spawnedParticle = InstantiateParticle(particle, rotation, originTransform, isFlipped);
        particle.instantiatedParticles.Add(spawnedParticle);

        if (!particle.isConstant)
            RemoveParticlesAfterTime(particle, originTransform);
        else spawnedParticle.transform.SetParent(originTransform);
        
        var particleRotation = particle.rotation;
        var flipOffset = new Vector3(0, 180, 0);
        
        if(isFlipped)
            particleRotation += flipOffset;
        
        if (particle.flipHorizontal) 
            particleRotation += flipOffset;
        
        spawnedParticle.transform.Rotate(particleRotation);
    }

    private GameObject InstantiateParticle(ParticleData particle, Quaternion rotation, Transform originTransform, bool flippedState = false)
    {
        var particlePrefab = Instantiate(particle.prefab, originTransform.position, rotation);
        
        var particleOffset = particle.offset;
        if (flippedState)
            particleOffset.x *= -1;
        
        particlePrefab.transform.Translate(particleOffset, Space.World);
        
        if (particlePrefab.HasComponent<VisualEffect>())
        {
            _newVFX = particlePrefab.GetComponent<VisualEffect>();
            _newVFX.Play();

            if (_newVFX.HasFloat(particle.lifeTime))
            {
                particle.defaultLifeTime = particle.newVFX.GetFloat(particle.lifeTime);
            }
        }
        else if (particlePrefab.HasComponent<ParticleSystem>())
        {
            _newParticleSystem = particlePrefab.GetComponent<ParticleSystem>();
            _newParticleSystem.Play();

            if (!_newParticleSystem.main.loop)
            {
                particle.defaultLifeTime = _newParticleSystem.main.duration;
            }
        }
        return particlePrefab;
    }

    private void RemoveParticlesAfterTime(ParticleData particle, Transform originTransform)
    {
        // StartCoroutine(StopParticle(particle.defaultLifeTime));
        // StartCoroutine(Remove(particle, particleObject));
        DestroyParticle(particle, originTransform);
    }

    IEnumerator Remove(ParticleData particle, GameObject particleObject)
    {
        yield return new WaitForSeconds(particle.defaultLifeTime + particle.pauzeTime);
        if (particleObject != null)
        {
            Destroy(particleObject);
            particle.instantiatedParticles.Remove(particleObject);
        }
    }
    public void DestroyParticle(int id, Transform originTransform)
    {
        if (id >= particles.Length) return;
        DestroyParticle(particles[id], originTransform);
    }

    private void DestroyParticle(ParticleData particle, Transform originTransform)
    {
        if (_newParticleSystem != null)
        {
            StartCoroutine(StopParticle(particle.defaultLifeTime));
        }
        
        var count = particle.instantiatedParticles.Count - 1;
        for (var i = count; i >= 0; i--)
        {
            var targetParticle = particle.instantiatedParticles[i];

            if (targetParticle == null) return;  

            if (particle.isConstant)
            {
                if (targetParticle.transform.parent.gameObject != originTransform.gameObject) continue;
            }
            
            particle.instantiatedParticles.RemoveAt(i);
            Destroy(targetParticle, particle.defaultLifeTime + particle.pauzeTime);
        }
    }

    IEnumerator StopParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (_newParticleSystem != null)
        {
            _newParticleSystem.Stop();
        }
    }

    IEnumerator CooldownTimer(float cooldownTime, ParticleData particle, Quaternion rotation, Transform originTransform)
    {
        if (cooldownTime > 0) yield return new WaitForSeconds(cooldownTime);
        SetInstantiateValues(particle, rotation, originTransform);
    }
}