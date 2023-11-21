using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SetActive(true);
    }
}
