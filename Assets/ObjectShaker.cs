using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShaker : MonoBehaviour
{
    [SerializeField] private float intensity = .5f;
    [SerializeField] private float frequency = .01f;
    private bool _isShaking;

    public void StartShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }

    private IEnumerator Shake(float duration)
    {
        StartCoroutine(ShakeTimer(duration));

        while(_isShaking)
        {
            var lastPos = transform.position;
            var newPos = lastPos;

            newPos.y += Random.Range(-intensity, intensity);

            transform.position = newPos;

            yield return new WaitForSeconds(frequency);

            transform.position = lastPos;
        }
    }

    private IEnumerator ShakeTimer(float duration)
    {
        _isShaking = true;

        yield return new WaitForSeconds(duration);
        
        _isShaking = false;
    }
}
