using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachine.PlayerInputHandlers
{
    public abstract class InputHandler : MonoBehaviour
    {
        public abstract void OnInput(InputAction.CallbackContext aContext);
    }
}
