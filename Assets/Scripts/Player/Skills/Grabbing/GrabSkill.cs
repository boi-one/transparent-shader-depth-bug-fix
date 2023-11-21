using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Points.PointTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Skills.Grabbing
{
    struct GrabpointWithParent
    {
        public GrabbablePoint grabpoint;
        public Grabbable parent;
    }
    public class GrabSkill : MonoBehaviour
    {
        [SerializeField] private float releaseForceMultiplier = 100f;
        [SerializeField] private Force grabForce;
        [SerializeField] private Force releaseForce;

        public UnityEvent onGrab = new UnityEvent();
        public UnityEvent onPullUp = new UnityEvent();
        public UnityEvent onGrabOrRelease = new UnityEvent();
        public UnityEvent onRelease =  new UnityEvent();

        [Header("OnRelease offset")]
        [SerializeField] private Vector2 releaseEdgeOffset;
        [SerializeField] private Vector3 releaseOffset;

        [Header("OnGrab offset")]
        [SerializeField] private Vector2 grabEdgeOffset;
        [SerializeField] private Vector2 grabWallOffset;
        [SerializeField] private Vector2 grabHangOffset;
        [SerializeField] private Vector2 grabNormalOffset;

        [SerializeField, Tooltip("GameObject we move to the grabbing point")]
        private GameObject parent;

        [SerializeField] private ForceBody forceBody;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        private static readonly int IsgrabbingAnimatorString = Animator.StringToHash("IsGrabbing");
        [SerializeField] private float releaseWaitingTime = .5f;
        [SerializeField] private float waitingTimeBeforeCanRelease = .5f;
        [SerializeField] private float timeBeforeCanGrabAgain = 2f;

        private GrabpointWithParent _lastGrabPoint;
        private bool _isGrabbing;
        private bool _canGrabOrRelease = true;

        private GrabbableCollector _grabbableCollector;

        private bool _isStaticParent;

        private void Awake()
        {
            _grabbableCollector = gameObject.GetOrAddComponent<GrabbableCollector>();
        }

        public bool OnGrabButtonPressed(InputAction.CallbackContext context)
        {
            if (!_canGrabOrRelease) return false;
            StartCoroutine(_isGrabbing ? Release() : Grab());
            return _isGrabbing;
        }

        public void ForceRelease(bool release = false)
        {
            if (_isGrabbing) StartCoroutine(Release(release));
        }

        private IEnumerator Grab()
        {
            var currentGrabPoint = GetGrabPoint();

            if (currentGrabPoint.grabpoint == null) yield break;
            var pointIsOccupied = currentGrabPoint.grabpoint.isOccupied;

            if (pointIsOccupied) yield break;

            onGrab.Invoke();
            onGrabOrRelease.Invoke();
            animator.SetBool(IsgrabbingAnimatorString, true);

            currentGrabPoint.grabpoint.isOccupied = true;
            _lastGrabPoint = currentGrabPoint;
            _isGrabbing = true;

            currentGrabPoint.parent.Grab(parent.gameObject);
            _isStaticParent = currentGrabPoint.parent.GetComponentInParent<ForceBody>() == null;
            if (!_isStaticParent)
            {
                forceBody.onCollisionEvent.AddListener(OnCollision);
            }
            else
            {
                forceBody.IsEnabled = false;
                UpdatePosition();
            }

            yield return new WaitForSeconds(waitingTimeBeforeCanRelease);
            _canGrabOrRelease = true;
        }

        private void OnCollision(CollisionForceEvent collisionForceEvent)
        {
            ForceRelease();
        }

        private void FixedUpdate()
        {
            if (_isGrabbing)
            {
                if (_lastGrabPoint.grabpoint.GameObject == null)
                    ForceRelease();
                else if (!_isStaticParent)
                    UpdatePosition();


            }
        }

        private void UpdatePosition()
        {
            Vector3 grabPosition = GetGrabPosition(_lastGrabPoint.grabpoint);
            if (!_isStaticParent)
            {
                grabForce.Direction = grabPosition - parent.transform.position;
                // voeg deze grabForce toe aan de forceBody
                forceBody.Add(grabForce);
            }
            else parent.transform.position = grabPosition;


            if (_lastGrabPoint.grabpoint.AutoOrient)
            {
                var scale = parent.transform.localScale;
                switch (_lastGrabPoint.grabpoint.GrabType)
                {
                    case GrabType.LEFTEDGE:
                        scale.x = 1;
                        break;
                    case GrabType.RIGHTEDGE:
                        scale.x = -1;
                        break;
                    default:
                        scale.x = _lastGrabPoint.grabpoint.GameObject.transform.lossyScale.x;
                        break;
                }
                parent.transform.localScale = scale;
            }
        }

        private IEnumerator Release(bool release = false)
        {
            _isGrabbing = false;
            _canGrabOrRelease = false;
            if (_lastGrabPoint.grabpoint == null) yield break;

            forceBody.onCollisionEvent.RemoveListener(OnCollision);

            onPullUp?.Invoke();
            onGrabOrRelease?.Invoke();
            onRelease?.Invoke();

            animator.SetBool(IsgrabbingAnimatorString, false);
            var velocity = forceBody.Velocity;

            yield return new WaitForSeconds(releaseWaitingTime);
            forceBody.IsEnabled = true;
            forceBody.ClearForces();
            forceBody.ResetGravity();

            var releaseForceDirection = velocity;
            releaseForceDirection.x *= releaseForceMultiplier;
            releaseForce.Direction = releaseForceDirection;

            forceBody.Add(releaseForce);

            forceBody.SetPosition(SetReleasePosition(_lastGrabPoint.grabpoint, release), Vector3.up);

            _lastGrabPoint.parent.Release();

            yield return new WaitForSeconds(timeBeforeCanGrabAgain);
            _canGrabOrRelease = true;

            _lastGrabPoint.grabpoint.isOccupied = false;

            yield break;
        }

        public void RemoteRelease()
        {
            StartCoroutine(Release());
        }

        private Vector2 SetReleasePosition(GrabbablePoint aGrabPoint, bool release = false)
        {
            var parentPosition = parent.transform.position;

            if (aGrabPoint == null) return parentPosition;

            switch (aGrabPoint.GrabType)
            {
                case GrabType.LEFTEDGE:
                    parentPosition += release ? -releaseOffset : (Vector3) releaseEdgeOffset;
                    break;

                case GrabType.RIGHTEDGE:
                    parentPosition += release ? releaseOffset : (Vector3) releaseEdgeOffset.InvertedX();
                    break;
            }
            return parentPosition;
        }

        private float GetDirectionalOffset(Vector2 offset)
        {
            return offset.x * MoveUtils.GetDirection(parent);
        }

        private Vector2 GetGrabPosition(GrabbablePoint aGrabablePoint)
        {
            if (aGrabablePoint == null) return parent.transform.position;
            Vector2 grabPoint;
            switch (aGrabablePoint.GrabType)
            {
                case GrabType.LEFTEDGE:
                    grabPoint = new Vector2(aGrabablePoint.Position.x + grabEdgeOffset.x, aGrabablePoint.Position.y - grabEdgeOffset.y);
                    break;
                case GrabType.RIGHTEDGE:
                    grabPoint = new Vector2(aGrabablePoint.Position.x - grabEdgeOffset.x, aGrabablePoint.Position.y - grabEdgeOffset.y);
                    break;
                case GrabType.HANG:
                    grabPoint = new Vector2(aGrabablePoint.Position.x - GetDirectionalOffset(grabHangOffset), aGrabablePoint.Position.y - grabHangOffset.y);
                    break;
                case GrabType.WALL:
                    grabPoint = new Vector2(aGrabablePoint.Position.x - grabWallOffset.x, aGrabablePoint.Position.y - grabWallOffset.y);
                    break;
                case GrabType.NORMAL:
                    grabPoint = new Vector2(aGrabablePoint.Position.x - GetDirectionalOffset(grabNormalOffset), aGrabablePoint.Position.y - grabNormalOffset.y);
                    break;
                default:
                    grabPoint = new Vector2(aGrabablePoint.Position.x, aGrabablePoint.Position.y);
                    break;
            }
            return grabPoint;
        }

        private Quaternion SetGrabRotation(GrabbablePoint aGrabablePoint)
        {
            if (aGrabablePoint == null) return parent.transform.rotation;
            switch (aGrabablePoint.GrabType)
            {
                case GrabType.LEFTEDGE:
                    RotatePlayerToRight();
                    return parent.transform.rotation;
                case GrabType.RIGHTEDGE:
                    RotatePlayerToLeft();
                    return parent.transform.rotation;
                default:
                    return parent.transform.rotation;
            }
        }

        private Quaternion SetReleaseRotation(GrabbablePoint aGrabablePoint)
        {
            return new Quaternion(0, 0, 0, 0);
        }

        // todo: verplaatsen. Dit hoort meer bij de grabbable of collector
        private GrabpointWithParent GetGrabPoint()
        {
            GrabpointWithParent grabPoint = new GrabpointWithParent();
            if (_grabbableCollector.list.Count <= 0) return grabPoint;

            Vector3 playerPos = transform.position;

            List<GrabpointWithParent> points = new List<GrabpointWithParent>();
            foreach (Grabbable grabbable in _grabbableCollector.list)
            {
                foreach (GrabbablePoint grabablePoint in grabbable.GetAllGrabbablePoints())
                {
                    points.Add(new GrabpointWithParent()
                    {
                        grabpoint = grabablePoint,
                        parent = grabbable
                    });
                }
            }

            var closest = points.OrderBy(grabpointParent => Vector2.Distance(grabpointParent.grabpoint.Position, playerPos)).ToList();

            foreach (GrabpointWithParent grabpointParent in closest)
            {
                if (Vector2.Distance(grabpointParent.grabpoint.Position, playerPos) > grabpointParent.grabpoint.Radius) continue;
                grabPoint = grabpointParent;
            }

            return grabPoint;
        }

        /// <summary>
        /// rotates parent by reversing its localScale
        /// </summary>
        private void RotatePlayerToRight()
        {
            var localScale = parent.transform.localScale;
            localScale = new Vector3(Math.Abs(localScale.x), localScale.y, localScale.z);
            parent.transform.localScale = localScale;
        }

        /// <summary>
        /// rotates parent by reversing its localScale
        /// </summary>
        private void RotatePlayerToLeft()
        {
            var localScale = parent.transform.localScale;
            localScale = new Vector3(-Math.Abs(localScale.x), localScale.y, localScale.z);
            parent.transform.localScale = localScale;
        }

        public bool  IsGrabbing => _isGrabbing;
    }
}
