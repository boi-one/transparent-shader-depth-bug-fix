using System;
using UnityEngine;

public class BounceSetting : MonoBehaviour
{
    [SerializeField] private float bounciness = 1;
    [SerializeField] private float bounceDecreaseMultiplier = 0.9f;
    [SerializeField, Tooltip("The amount of bounce when collision with other forcebody with a bounce force")] private float onBounceBounciness = 0.5f;
    [SerializeField] private float minBouncesUntilDecrease = 1;

    public float Bounciness => bounciness;

    public float BounceDecreaseMultiplier => bounceDecreaseMultiplier;

    public float OnBounceBounciness => onBounceBounciness;

    public float MinBouncesUntilDecrease => minBouncesUntilDecrease;
    
    public bool CanBounce { get; set; }
}
