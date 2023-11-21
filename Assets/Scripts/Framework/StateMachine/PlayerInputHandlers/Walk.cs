using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachine.PlayerInputHandlers
{
    [RequireComponent(typeof(Rigidbody))]
    public class Walk : InputHandler
    {
        [SerializeField] float speed = 25f;
        [SerializeField] float maxVelocityChange = 10.0f;
        [SerializeField] float stickDeadZone;

        private Rigidbody _rigidbody;
        private static readonly int InputX = Animator.StringToHash("InputX");
        private static readonly int Velocity = Animator.StringToHash("Velocity");

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void OnInput(InputAction.CallbackContext aContext)
        {
            OnInput(aContext.ReadValue<Vector2>());
        }

        public void OnInput(Vector2 dir)
        {
            GetComponentInChildren<Animator>().SetFloat(InputX, Math.Abs(dir.x));
            
            if (Mathf.Abs(dir.x) < stickDeadZone)
            {
                return;
            }
            if (dir.x != 0) GetComponent<PlayerRotation>().RotatePlayer(dir);
            
            Vector3 deltaVelocity = new Vector3(dir.x, 0, 0);
            deltaVelocity = transform.TransformDirection(deltaVelocity);
            deltaVelocity *= speed;

            Vector3 velocity = _rigidbody.velocity;
            Vector3 velocityChange = (deltaVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            velocityChange.z = 0;
        
            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            
            GetComponentInChildren<Animator>().SetFloat(Velocity,Mathf.Abs(_rigidbody.velocity.x));
        }
    }
}
