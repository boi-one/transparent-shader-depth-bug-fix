using System;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    [Header("Raycast")] 
    [SerializeField]private Transform groundCheck;
    [SerializeField]private LayerMask groundMask;
    [SerializeField]private float rayLength = 1.5f;

    private readonly UnityEvent _onGroundLand = new UnityEvent();
    private readonly UnityEvent _onGroundExit = new UnityEvent();
    private readonly UnityEvent<bool, bool> _onGroundStateChange = new UnityEvent<bool, bool>();

    private bool _isGrounded;
    
    public bool GroundCheck()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, rayLength , groundMask);
    }

    private void Update()
    {
        bool previousState = _isGrounded;
        
        _isGrounded = GroundCheck();

        if (_isGrounded == previousState) return;
        if (_isGrounded) _onGroundLand.Invoke();
        else _onGroundExit?.Invoke();
        
        _onGroundStateChange?.Invoke(previousState, _isGrounded);
    }

    public bool IsGrounded => _isGrounded;

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.magenta; 
        Gizmos.DrawRay(groundCheck.position, Vector3.down);
    }
}

