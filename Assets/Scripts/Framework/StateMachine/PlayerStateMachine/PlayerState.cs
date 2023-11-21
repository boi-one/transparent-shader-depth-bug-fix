using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using StateMachine.PlayerInputHandlers;
using UnityEngine.Events;


namespace StateMachine.PlayerStateMachine
{
    public abstract class PlayerState : StateBehaviour
    {
        [SerializeField] protected List<PlayerStateAction> Performed = new List<PlayerStateAction>();
        [SerializeField] protected List<PlayerStateAction> Canceled = new List<PlayerStateAction>();
        [SerializeField] protected List<PlayerStateAction> Started = new List<PlayerStateAction>();
            
        [SerializeField] protected InputParser inputParser;

        public override void OnEnter()
        {
            foreach (var playerStateAction in Performed)
                inputParser.PlayerControlsActions[playerStateAction.action].performed += playerStateAction.inputHandler.OnInput;
            foreach (var playerStateAction in Canceled)
                inputParser.PlayerControlsActions[playerStateAction.action].canceled += playerStateAction.inputHandler.OnInput;
            foreach (var playerStateAction in Started)
                inputParser.PlayerControlsActions[playerStateAction.action].started += playerStateAction.inputHandler.OnInput;

        }

        public override void OnExit()
        {
            foreach (var playerStateAction in Performed)
                inputParser.PlayerControlsActions[playerStateAction.action].performed -= playerStateAction.inputHandler.OnInput;
            foreach (var playerStateAction in Canceled)
                inputParser.PlayerControlsActions[playerStateAction.action].canceled -= playerStateAction.inputHandler.OnInput;
            foreach (var playerStateAction in Started)
                inputParser.PlayerControlsActions[playerStateAction.action].started -= playerStateAction.inputHandler.OnInput;
        } 
    }
}