using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class AnimationFlip : MonoBehaviour
{
    [SerializeField] private string animationParameterName = "RunSpeedMultiplier";
    [SerializeField] private UnityEvent<string ,float> onAnimationFlip = new UnityEvent<string, float>();

    private bool _animationIsInverted;

    public bool CanFlipAnimation { get; set; }

    public bool IsPlayingAnimation { get; set; }

    public int Direction { get; set; }

    public void CheckDirection(int lookDirection)
    {
        if (CanFlipAnimation && IsPlayingAnimation)
        {
            if (lookDirection != Direction && !_animationIsInverted)
            {
                ChangeAnimationSpeed(true);
            }

            if (lookDirection == Direction && _animationIsInverted)
            {
                ChangeAnimationSpeed(false);
            }
        }
    }

    private void ChangeAnimationSpeed(bool inverted)
    {
        onAnimationFlip?.Invoke(animationParameterName, inverted ? -1 : 1);
        _animationIsInverted = inverted;
    }

    public void ResetAnimationSpeed()
    {
        if(_animationIsInverted) ChangeAnimationSpeed(false);
    }
}
