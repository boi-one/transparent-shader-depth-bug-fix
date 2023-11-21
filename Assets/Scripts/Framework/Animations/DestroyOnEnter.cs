using UnityEngine;

public class DestroyOnEnter : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Destroy(animator.gameObject, stateInfo.length);
    }
    
}
