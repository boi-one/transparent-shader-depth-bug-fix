using Skills.Grabbing;
using UnityEngine;

namespace Points.PointTypes
{
    public class GrabbablePoint : Point
    {
        [SerializeReference] private GrabType _grabType = GrabType.NORMAL;
        [SerializeReference] private bool _autoOrient = true;
        public bool isOccupied;
        
        public GrabType GrabType
        {
            get => _grabType;
            set => _grabType = value;
        }
        
        public bool AutoOrient
        {
            get => _autoOrient;
        }

        public GrabbablePoint(Vector3 aPosition) : base(aPosition)
        {
        }
        
        public GrabbablePoint(Vector3 aPosition, GameObject aGameObject) : base(aPosition, aGameObject)
        {
        }
    }
}