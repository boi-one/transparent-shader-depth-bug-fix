using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class RotateAndScale : MonoBehaviour
{
   [SerializeField] private float speed = 0.25f;
   [SerializeField] private float rotationAngleY = 90;
   [SerializeField] private float radius = 5;
   [SerializeField] private float animationTime = 0.6f;

   private float _finalSize;
   private float _originalLocalScale;
   
   private bool _isScalingUp = false;
   private bool _isScalingDown = false;
   private bool _finishedAnimation = false;
   
   private float _startTime;

   private void Start()
   {
       _originalLocalScale = transform.localScale.x;
       _finalSize = radius * 2;
   }

   // Update is called once per frame
    void Update()
    {
        if (_isScalingUp)
        {
            ScaleToMaxSize();
        }

        if (_isScalingDown)
        {
            ScaleToOriginalSize();
        }

        float rotationY = Mathf.SmoothStep(0, rotationAngleY, Mathf.Repeat(Time.time * speed, 1));
        transform.rotation = Quaternion.Euler(0,rotationY,0);
    }

    private void ScaleToMaxSize()
    {
        ScaleGameObject(_originalLocalScale, _finalSize);

        if (IsDoneScaling(GetCurrentScale(), _finalSize))
        {
            _isScalingUp = false;
            _isScalingDown = true;
            _startTime = Time.time;
        }
    }

    private void ScaleToOriginalSize()
    {
        ScaleGameObject(_finalSize, _originalLocalScale);

        if (IsDoneScaling(_originalLocalScale, GetCurrentScale()))
        {
            _isScalingDown = false;
            _finishedAnimation = true;
        }
    }

    /// <summary>
    /// This is a function to check if the gameObject this script is attached to is done scaling.
    /// </summary>
    /// <param name="currentScale">This gameObject's "localScale", this does not have to be in particular the current scale.
    /// example: in cases where you want to revert the scale (check method: ScaleToOriginalSize()) </param>
    /// 
    /// <param name="expectedScale"> This gameObject's expected "localScale", this does not have to be in particular the expected scale.
    /// example: in cases where you want to revert the scale (check method: ScaleToOriginalSize())</param>
    /// <returns>True currentScale is equal or higher then expectedScale</returns>
    private bool IsDoneScaling(float currentScale, float expectedScale)
    {
        return currentScale >= expectedScale - 0.0001f;
    }
    
    
    private float GetCurrentScale()
    {
        return transform.localScale.x;
    }
    
    private void ScaleGameObject(float beginSize, float endSize)
    {
        float newScale = Mathf.Lerp(beginSize, endSize, GetAnimationTime());
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
    
    public void SetRadius(float newRadius)
    {
        this.radius = newRadius;
    }

    public void PlayAnimation()
    {
        _isScalingUp = true;
        _startTime = Time.time;
        
        StartCoroutine(SuspendAnimation());
    }
    
    IEnumerator SuspendAnimation()
    {
        while (!_finishedAnimation)
        {
            yield return null;
        }

        _finishedAnimation = false;
        
        this.gameObject.SetActive(false);
    }


    private float GetAnimationTime()
    {
        return (Time.time - _startTime) / (animationTime / 2);
    }

}
