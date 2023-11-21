using UnityEngine;
using System;

namespace StateMachine.PlayerInputHandlers
{
    public class PlayerRotation : MonoBehaviour
    {
        public void RotatePlayer(Vector2 dir)
        {
            Vector3 localScale = transform.localScale;
            float newX = Mathf.Sign(dir.x) * Math.Abs(dir.x);
            newX = newX > 0 ? 1 : -1;
            
            transform.localScale = new Vector3(newX, localScale.y, localScale.z);
        }
    }
}