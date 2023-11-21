using System;
using StateMachine.PlayerStateMachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace StateMachine.PlayerInputHandlers
{
    [RequireComponent(typeof(Rigidbody))]
    public class Jump : InputHandler
    {
        [SerializeField] private UnityEvent onJump = new UnityEvent();
        [SerializeField] private float jumpForce;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnInput(InputAction.CallbackContext aContext)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(new Vector3(_rigidbody.velocity.x, jumpForce, 0), ForceMode.VelocityChange);
            onJump?.Invoke();

            GetComponentInChildren<StateMachine>().SetBool("Jumping", true);
        }
    }
}
