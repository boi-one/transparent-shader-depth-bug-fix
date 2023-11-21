using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorControls : MonoBehaviour
{
    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();

    public void SetBool(string input) => Parse(input, bool.Parse, _animator.SetBool);
    public void SetFloat(string input) => Parse(input, float.Parse, _animator.SetFloat);
    public void SetInt(string input) => Parse(input, int.Parse, _animator.SetInteger);
    
    void Parse<T>(string input, Func<string, T> parser, Action<string, T> targetMethod)
    {
        if (input.Split(':').Length != 2) return;
        var paramName = input.Split(':')[0];
        var value = parser(input.Split(':')[1]);
        targetMethod(paramName, value);
    }
}