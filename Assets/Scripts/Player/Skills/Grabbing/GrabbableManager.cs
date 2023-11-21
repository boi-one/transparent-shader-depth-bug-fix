using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills.Grabbing
{
    public class GrabbableManager : MonoBehaviour
    {
        private Grabbable[] _grabbables;

        public void TurnOffOccupied()
        {
            _grabbables = FindObjectsOfType<Grabbable>();

            foreach (var grabbable in _grabbables)
            {
                grabbable.ToggleGrabPoints(false);
            }
        }
    }
}


