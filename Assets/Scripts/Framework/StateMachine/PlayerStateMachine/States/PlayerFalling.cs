using StateMachine.PlayerInputHandlers;
using UnityEngine;

namespace StateMachine.PlayerStateMachine.States
{
    public class PlayerFalling : PlayerState
    {
        [SerializeField] private Animator animator;
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        
        private static readonly int Falling = Animator.StringToHash("isFalling");

        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetBool(Falling, true);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool(Falling, false);
        }


        private void FixedUpdate()
        {
            Vector2 moveInput = inputParser.PlayerControlsActions["Move"].ReadValue<Vector2>();
            GetComponentInParent<Walk>().OnInput(moveInput);
        }
        
        private void Update()
        {
            StateMachine.SetFloat("VelocityX", GetComponentInParent<Rigidbody>().velocity.x);
            StateMachine.SetFloat("VelocityY", GetComponentInParent<Rigidbody>().velocity.y);
            StateMachine.SetBool("isGrounded", GetComponentInParent<GroundChecker>().GroundCheck());
            
            animator.SetBool(IsGrounded, GetComponentInParent<GroundChecker>().GroundCheck());
        }
    }
}