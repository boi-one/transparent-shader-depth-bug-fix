using System;
using System.Collections.Generic;
using System.Linq;
using Points.PointTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Skills.Grabbing
{
    [RequireComponent(typeof(PointCreator))]
    public class Grabbable : MonoBehaviour   
    {
        public UnityEvent onGrab = new UnityEvent();
        public UnityEvent onRelease = new UnityEvent();
        public PointCreator _pointCreator;
        public GameObject parent;

        private GameObject _currentPlayer;
        [SerializeField] private string impactForceName = "Grab";


        private void Awake()
        {
            _pointCreator = GetComponent<PointCreator>();
            if (parent == null)
                parent = gameObject;
        }

        public Vector2 GetGrabPoint(Vector2 fromPosition)
        {
            var grabList = GetAllGrabbablePoints();
            var closest = grabList.OrderBy(trans => Vector2.Distance(trans.Position, fromPosition)).First();
            return closest.Position;
        }

        public void Grab(GameObject player)
        {
            _currentPlayer = player;
            onGrab?.Invoke();
            AddGrabForce();
        }

        private void AddGrabForce()
        {
            var forceBody = parent.GetComponent<ForceBody>();
            if (forceBody == null) return;
            var impactForce = ForceLibrary.Get(impactForceName);
            if(impactForce != null)
                forceBody.Add(impactForce);
        }

        public void Release()
        {
            onRelease?.Invoke();
        }

        public List<GrabbablePoint> GetAllGrabbablePoints()
        {
            return _pointCreator.pointHolder.GetList().OfType<GrabbablePoint>().ToList();
        }

        public void ToggleGrabPoints(bool isOn)
        {
            var points = GetAllGrabbablePoints();
            var l = points.Count;

            for (var i = 0; i < l; i++)
            {
                points[i].isOccupied = isOn;
            }
        }

        public void ReleasePlayer()
        {
            if (_currentPlayer == null) return;
            _currentPlayer.GetComponentInChildren<GrabSkill>().RemoteRelease();
        }
    }
}