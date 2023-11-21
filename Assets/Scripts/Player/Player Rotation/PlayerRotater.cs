using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class PlayerRotater : MonoBehaviour
{
    [SerializeField] private UnityEvent<int> playerLookDirectionChanged = new UnityEvent<int>();
    private int _currentDirection = 1;

    public bool CanRotatePlayer { get; set; }

    private int CurrentDirection
    {
        get => _currentDirection;
        set
        {
            if(_currentDirection != value) playerLookDirectionChanged?.Invoke(value);
            _currentDirection = value;
        }
    }

    public void OnDirectionChanged(Vector2 direction)
    {
       if(direction.x < 0) UpdatePlayerDirectionAim(-1);
       else if(direction.x > 0) UpdatePlayerDirectionAim(1);
    }

    private void UpdatePlayerDirectionAim(int direction)
    {
        if (!CanRotatePlayer) return;
        
        SetDirection(direction);
    }

    private void SetDirection(int newDirection)
    {
        transform.localScale = new Vector3(newDirection, 1, 1);

        CurrentDirection = newDirection;
    }
}
