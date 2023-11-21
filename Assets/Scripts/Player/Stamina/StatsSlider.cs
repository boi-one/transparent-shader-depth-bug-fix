using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatsSlider : MonoBehaviour
{
    public Slider statsSlider;
    [SerializeField] private float staminaSlidingTime;
    [SerializeField] private float fadeTime;
    private StaminaPool _staminaComponent;
    [SerializeField] private float fadeWaitTime;

    // fade
    [SerializeField] private float fadeProgress;

    public bool ShouldChange { private get; set; }
    public bool IsFull { private get; set; }

    private void Start()
    {
        InitStaminaSlider();
        SetShouldChange(true);
        ChangeVisibility(false);
    }

    private void Update()
    {
        if (_staminaComponent == null) Destroy(this.gameObject);
        if (!ShouldChange) return;
        UpdateFade();
    }

    private void UpdateFade()
    {
        if (fadeProgress >= 1)
        {
            fadeProgress = 0;
            ShouldChange = false;
            return;
        }
        fadeProgress += Time.deltaTime / fadeTime;

        ChangeVisibility(false);
    }

    public void UpdateStaminaBar()
    {
        statsSlider.value = Mathf.Lerp(statsSlider.value, _staminaComponent.Stamina, staminaSlidingTime);
    }
    
    private IEnumerator WaitAndChange(bool shouldToChange)
    {
        yield return new WaitForSeconds(fadeWaitTime);
        if (!IsFull) yield break;
        ShouldChange = shouldToChange;
    }

    public void SetShouldChange(bool shouldChange)
    {
        StartCoroutine(WaitAndChange(shouldChange));
    }

    public void ResetFade()
    {
        fadeProgress = 0;
    }

    public void InitStaminaSlider()
    {
        var canvas = FindObjectOfType<Canvas>();
        var statsTransform = statsSlider.transform;
        _staminaComponent = GetComponentInParent<StaminaPool>();
        statsSlider.value = statsSlider.maxValue;
        statsTransform.parent = canvas.transform;
        statsTransform.position = new Vector3(0, +20, 0);
        MoveStaminaBar(canvas);
    }

    private void MoveStaminaBar(Canvas canvas)
    {
        var getAllStaminaBars = canvas.GetComponentsInChildren<StatsSlider>();
        for (var i = 1; i < getAllStaminaBars.Length; i++)
        {
            getAllStaminaBars[i].statsSlider.transform.position = new Vector3(0, +40, 0);
        }
    }

    public void ChangeVisibility(bool shouldBeSeen)
    {
        var images = gameObject.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            var clr = image.color;
            var newColor = new Color(clr.r, clr.g, clr.b, Mathf.Lerp(shouldBeSeen ? 0 : 1, shouldBeSeen ? 1 : 0, fadeProgress));
            image.color = newColor;
        }
    }
}