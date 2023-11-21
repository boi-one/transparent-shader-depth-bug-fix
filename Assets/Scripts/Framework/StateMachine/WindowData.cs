using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StateMachine
{
    public class WindowData
    {
        public Rect rect;
        public Object source;
        public Type type;

        public WindowData(Rect rect)
        {
            this.rect = rect;
        }
    }
}
