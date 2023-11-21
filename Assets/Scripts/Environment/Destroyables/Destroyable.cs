using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour, IDamagable
{
    [SerializeField] private Shader shaderEffect;
    [SerializeField] private float effectDuration = 0.15f;

    private Shader _startShader;

    public UnityEvent onHit = new UnityEvent();

    private void Awake()
    {
        _startShader = gameObject.GetComponent<Renderer>().material.shader;
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(nameof(FlashEffect));
        onHit.Invoke();
    }

    private IEnumerator FlashEffect()
    {
        gameObject.GetOrAddComponent<Renderer>().material.shader = shaderEffect;
        yield return new WaitForSeconds(effectDuration);
        gameObject.GetOrAddComponent<Renderer>().material.shader = _startShader;
    }
}