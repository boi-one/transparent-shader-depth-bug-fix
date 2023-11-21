using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LayerLerper : MonoBehaviour
{
    public float lerpSpeed;
    public string layerName;

    private int _layerValue;

    private void OnEnable()
    {
        GetComponent<Animator>().SetLayerWeight(GetComponent<Animator>().GetLayerIndex(layerName), _layerValue);
    }

    public void EnableLayer()
    {
        _layerValue = 1;
        if (!this.isActiveAndEnabled) return;
        StartCoroutine(StartLerp(1, 0));
    }

    public void DisableLayer()
    {
        _layerValue = 0;
        if (!this.isActiveAndEnabled) return;
        StartCoroutine(StartLerp(0, 1));
    }

    private IEnumerator StartLerp(int targetValue, int startValue)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * lerpSpeed;
            GetComponent<Animator>().SetLayerWeight(GetComponent<Animator>().GetLayerIndex(layerName), Mathf.Lerp(startValue, targetValue, percent));
        }
        GetComponent<Animator>().SetLayerWeight(GetComponent<Animator>().GetLayerIndex(layerName), targetValue);
        yield return null;
    }
}
