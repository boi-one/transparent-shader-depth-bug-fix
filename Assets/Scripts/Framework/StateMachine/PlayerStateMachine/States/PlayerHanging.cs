using UnityEngine;

namespace StateMachine.PlayerStateMachine.States
{
    public class PlayerHanging : PlayerState
    {
        [SerializeField] private Animator animator;
        
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        
        private void Update()
        {
            StateMachine.SetFloat("VelocityX", GetComponentInParent<Rigidbody>().velocity.x);
            StateMachine.SetFloat("VelocityY", GetComponentInParent<Rigidbody>().velocity.y);
            StateMachine.SetBool("isGrounded", GetComponentInParent<GroundChecker>().GroundCheck());
            
            animator.SetBool(IsGrounded, GetComponentInParent<GroundChecker>().GroundCheck());
        }
    }
}