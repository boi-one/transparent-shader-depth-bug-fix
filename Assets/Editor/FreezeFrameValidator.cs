using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class FreezeFrameValidator : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private string namePrefix = " *";
    [SerializeField] private float defaultExitTime = 1f;
 
    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
        var animatorController = _animator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null) return;
        
        var stateMachine = animatorController.layers[0].stateMachine;
        

        CheckStateMachine(stateMachine);

    }

    private void CheckStateMachine(AnimatorStateMachine stateMachine)
    {
        var states = stateMachine.states;
        foreach (var state in states)
        {
            if (state.state.behaviours.Length == 0) continue;
            var behaviour = state.state.behaviours[0];
            
            if (behaviour is FreezeFrameState)
            {
                if (!state.state.name.EndsWith(namePrefix))
                {
                    Debug.LogWarning("Animator State name changed for " + state.state.name);
                    state.state.name += namePrefix;
                }
                var freezeFrameBehaviour = behaviour as FreezeFrameState;
                var firstTransition = state.state.transitions[0];
                if (firstTransition.exitTime != 0.1f && freezeFrameBehaviour.FrameId == FreezeFrameIds.FirstFrame)
                {
                    Debug.LogWarning("Animator exittime for " + state.state.name + "does not match exittime rule for FirstFrame freezeframes");
                } else if (firstTransition.exitTime != 1.1f && freezeFrameBehaviour.FrameId == FreezeFrameIds.LastFrame)
                {
                    Debug.LogWarning("Animator exittime for " + state.state.name + "does not match exittime rule for LastFrame freezeframes");
                }
            }
        }

        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            CheckStateMachine(subStateMachine.stateMachine);
        }
    }
}