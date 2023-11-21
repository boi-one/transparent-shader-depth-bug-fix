using System;
using StateMachine.PlayerInputHandlers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachine.PlayerStateMachine.States
{
    public class PlayerAttack : PlayerState
    {
        [SerializeField] private Animator animator;
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        
        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<StateMachine>().SetBool("Attacking", false);
        }

        private void Update()
        {
            StateMachine.SetFloat("VelocityX", GetComponentInParent<Rigidbody>().velocity.x);
            StateMachine.SetFloat("VelocityY", GetComponentInParent<Rigidbody>().velocity.y);
            StateMachine.SetBool("isGrounded", GetComponentInParent<GroundChecker>().GroundCheck());
            
            animator.SetBool(IsGrounded, GetComponentInParent<GroundChecker>().IsGrounded);
        }

        private void FixedUpdate()
        {
            Vector2 moveInput = inputParser.PlayerControlsActions["Move"].ReadValue<Vector2>();
            GetComponentInParent<Walk>().OnInput(moveInput);
        }
    }
}