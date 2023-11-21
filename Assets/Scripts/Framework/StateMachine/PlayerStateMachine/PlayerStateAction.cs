using System;
using StateMachine.PlayerInputHandlers;

namespace StateMachine.PlayerStateMachine
{
    [Serializable]
    public class PlayerStateAction
    {
        public string action;
        public InputHandler inputHandler;
    }
}
