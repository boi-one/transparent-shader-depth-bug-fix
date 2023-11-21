using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaminaPool : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float stamina = 100f;
    private float _maxStamina;

    [Header("Regeneration")]
    [SerializeField] private bool canAutoRegenerate = true;
    [SerializeField] private float regenerationWaitTime = 2.5f;
    [SerializeField] private float exhaustedRegenerationMultiplier;
    [SerializeField] private float regenerationStep = 0.4f;
    private float _exhaustedRegenerationWaitTime;
    private Coroutine _regenerationPause;
    private bool _isRegenerating;
    private bool _hasStamina;

    [Header("Events")]
    [SerializeField] private UnityEvent onStaminaEmpty = new UnityEvent();
    [SerializeField] private UnityEvent onStaminaDeplete = new UnityEvent();
    [SerializeField] private UnityEvent onStaminaIncrease = new UnityEvent();
    [SerializeField] private UnityEvent onStaminaFull = new UnityEvent();
    void Awake()
    {
        _exhaustedRegenerationWaitTime = regenerationWaitTime * exhaustedRegenerationMultiplier;
        _maxStamina = Stamina;
        _hasStamina = true;
    }

    private void Start()
    {
        onStaminaFull?.Invoke();
    }

    public float Stamina => stamina;

    private void FixedUpdate()
    {
        if (!_isRegenerating) return;
        _hasStamina = true;
        IncreaseStamina(regenerationStep, _maxStamina); // Idle stamina regeneration
    }

    public void DepleteStamina(float staminaChange)
    {
        if (staminaChange <= 0) return;
        onStaminaDeplete.Invoke();
        
        var previousStamina = stamina;
        
        stamina -= staminaChange;

        if (stamina <= staminaChange) stamina = 0;

        if (previousStamina != stamina) onStaminaDeplete?.Invoke();

        if (stamina == 0)
        { 
            _hasStamina = false;
            onStaminaEmpty?.Invoke();
        }
        ResetDebounce();
    }

    private void ResetDebounce()
    {
        if (!canAutoRegenerate) return;
        if (_regenerationPause != null) StopCoroutine(_regenerationPause);
        _isRegenerating = false;
        _regenerationPause = StartCoroutine(_hasStamina ? PauseRegeneration(regenerationWaitTime) : PauseRegeneration(_exhaustedRegenerationWaitTime));
    }

    private void IncreaseStamina(float staminaChange, float maxStamina)
    {
        var previousStamina = stamina;
        stamina += staminaChange;
        stamina = Mathf.Min(stamina, maxStamina);
        if (previousStamina == stamina) return;
        if (stamina == maxStamina) onStaminaFull?.Invoke();
        onStaminaIncrease?.Invoke();
    }
    
    private IEnumerator PauseRegeneration(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);

        _isRegenerating = true;
    }
}
