using StateMachine.PlayerInputHandlers;
using UnityEngine;

namespace StateMachine.PlayerStateMachine.States
{
    public class PlayerJump : PlayerState
    {
        [SerializeField] private Animator animator;
        
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly float JumpPeakTolarance = 0.1f;
        private static readonly int OnJumpPeak = Animator.StringToHash("OnJumpPeak");
        private static readonly int OnJumpStart = Animator.StringToHash("OnJumpStart");

        private void FixedUpdate()
        {
            Vector2 moveInput = inputParser.PlayerControlsActions["Move"].ReadValue<Vector2>();
            GetComponentInParent<Walk>().OnInput(moveInput);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetTrigger(OnJumpStart);
            StateMachine.SetBool("Jumping", false);
        }

        private void Update()
        {
            StateMachine.SetFloat("VelocityX", GetComponentInParent<Rigidbody>().velocity.x);
            StateMachine.SetFloat("VelocityY", GetComponentInParent<Rigidbody>().velocity.y);
            StateMachine.SetBool("isGrounded", GetComponentInParent<GroundChecker>().GroundCheck());
            
            animator.SetBool(IsGrounded, GetComponentInParent<GroundChecker>().GroundCheck());
            CheckForJumpPeak();
        }
        
        private void CheckForJumpPeak()
        {
            if (GetComponentInParent<Rigidbody>().velocity.y < JumpPeakTolarance && !GetComponentInParent<GroundChecker>().IsGrounded)
            {
                animator.SetTrigger(OnJumpPeak);
            }
        }
    }
}
