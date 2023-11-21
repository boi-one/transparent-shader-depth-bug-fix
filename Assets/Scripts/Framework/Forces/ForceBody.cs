using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class ForceBody : MonoBehaviour
{
    private static Dictionary<ForceModifiers, Modifier> _modifiers = new Dictionary<ForceModifiers, Modifier>()
    {
        {ForceModifiers.Stack, new Stack()}
    };

    public List<IForceable> PausedForceables = new List<IForceable>();
    public List<IForceable> ActiveForceables = new List<IForceable>();
    public List<IForceable> GravityForceables = new List<IForceable>();
    public List<IForceable> CurrentForceables = new List<IForceable>();

    public Dictionary<IForceable, CallbackConfig> callbacks = new Dictionary<IForceable, CallbackConfig>();

    [Header("Events")]

    public UnityEvent<CollisionForceEvent> onCollisionEvent = new UnityEvent<CollisionForceEvent>();
    [SerializeField] private UnityEvent<ForceBody, RaycastHit> onCollision = new UnityEvent<ForceBody, RaycastHit>();
    public UnityEvent<IForceable, List<IForceable>> onForceAdded = new UnityEvent<IForceable, List<IForceable>>();
    public UnityEvent<IForceable, List<IForceable>> onForceRemoved = new UnityEvent<IForceable, List<IForceable>>();
    public UnityEvent<List<IForceable>[]> onForcesCleared = new UnityEvent<List<IForceable>[]>();
    public UnityEvent<List<IForceable>, List<IForceable>> onForcesConcatenated = new UnityEvent<List<IForceable>, List<IForceable>>();
    

    [Header("Settings")]
    [SerializeField] private bool useGravity = true;
    [SerializeField] private float gravityScale = 1;
    [SerializeField] private float mass = 1;

    public float radius = 1;
    public bool hasGravity = true;

    [SerializeField] private float collisionCastDistance = 10;

    [Header("Custom collision layers")]
    [SerializeField] private LayerMask ignoredCollisionLayers;
    [SerializeField] private LayerMask collisionLayers;

    private readonly List<Vector3> _castDirections = new List<Vector3>()
    {
        Vector3.up,
        Vector3.left,
        Vector3.right
    };

    private ForceSystem _forceSystem;

    private bool _hasCancellingForce;
    private bool _hasPausingForce;
    private bool _isEnabled = true;

    public LayerMask CollisionLayer { get; private set; }

    public bool UseGravity
    {
        get => useGravity;
        set => useGravity = value;
    }

    public float GravityScale
    {
        get => gravityScale;
        set => gravityScale = value;
    }

    public float Mass
    {
        get => mass;
        set => mass = value;
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        _forceSystem = ForceSystem.Instance;
        _forceSystem.AddBody(this);
        if (useGravity) AddGravity();
    }

    private void Update()
    {
        CollisionLayer = (_forceSystem.GroundLayer ^ ignoredCollisionLayers) | collisionLayers;
    }

    private void OnDestroy()
    {
        if(_forceSystem != null)_forceSystem.RemoveBody(this);
    }

    public void OnCollision(RaycastHit hit)
    {
        onCollision?.Invoke(this, hit);
    }

    public void OnCollisionEvent(CollisionForceEvent collisionEvent)
    {
        onCollisionEvent?.Invoke(collisionEvent);
    }

    private void AddGravity()
    {
        Add(new Force()
        {
            Direction = new Vector3(0, 0, 0),
            BlendType = BlendTypes.Blend,
            Type = ForceTypes.Gravity,
            Id = ForceIds.Gravity
        });
    }

    public IForceable Add(Force newForce, CallbackConfig callbackConfig = null)
    {
        if (_hasCancellingForce && newForce.Type != ForceTypes.Gravity)
        {
            callbackConfig?.OnCancel?.Invoke();
            return newForce;
        }

        if (newForce.BlendType == BlendTypes.RemoveOthers && !_hasPausingForce)
        {
            ClearForces();
        }

        if (newForce.BlendType == BlendTypes.PauseOthers && !_hasPausingForce)
        {
            DeactivateForces();
        }

        var targetCollection = GetTargetCollection(newForce);

        newForce.UpdateSurface();
        if (newForce.IsUniqueId)
        {
            Remove(newForce.Id);
        }

        ApplyModifier(newForce);

        IForceable targetForce = newForce;
        if (newForce.Type == ForceTypes.Default)
        {
            targetForce = ConvertToForcetween(newForce);
            if (callbackConfig != null)
            {
                callbacks.Add(targetForce, callbackConfig);
            }
        }
        else if(newForce.Type == ForceTypes.SingleAddition && callbackConfig != null)
        {
            callbacks.Add(targetForce, callbackConfig);
        }
        else
        {
            ConvertToFrameStepForce(newForce);
        }

        targetCollection.Add(targetForce);
        onForceAdded?.Invoke(targetForce, targetCollection);
        UpdateState();
        return targetForce;
    }

    /// <summary>
    ///     <para>Geef een Vector3 mee waar de forcebody naartoe moet.
    ///     De 2de vector kan je een voorkeur mee geven naar welke kant het beste is, down is niet aangeraden maar kan wel.
    ///     Default kan Vector3.Zero</para>
    /// </summary>
    public void SetPosition(Vector3 targetPosition, Vector3? desiredDirection = null)
    {
        transform.position = targetPosition;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, _forceSystem.GroundLayer);
        if (hitColliders.Length <= 0) return;

        var hit = ResolveCollision(hitColliders, desiredDirection);
        if (hit == null) return;

        transform.position = hit.Value.position + hit.Value.desiredDirection.normalized * radius;
    }

    private (Vector3 position, Vector3 desiredDirection)? ResolveCollision(Collider[] hitColliders, Vector3? desiredDirection)
    {
        var currentCollider = hitColliders[0].gameObject;

        if (desiredDirection == null || desiredDirection == Vector3.zero)
        {
            return GetColliderExit(currentCollider);
        }

        var hit = GetClosestHit((Vector3) desiredDirection, currentCollider);
        if (hit == null) return null;

        return (((RaycastHit)hit).point, (Vector3)desiredDirection);
    }

    private (Vector3 position, Vector3 desiredDirection)? GetColliderExit(GameObject currentCollider)
    {
        var hits = new List<(Vector3 direction, RaycastHit hit)>();

        foreach (Vector3 direction in _castDirections)
        {
            var closestHit = GetClosestHit(direction, currentCollider);
            if(closestHit == null) continue;

            hits.Add((direction, (RaycastHit)closestHit));
        }

        if (hits.Count == 0) return null;

        var allDistances = hits.Select(valueTuple => valueTuple.hit.distance).ToList();

        var nearestIndex = allDistances.IndexOf(allDistances.Max());
        var nearestHit = hits[nearestIndex];

        return (nearestHit.hit.point, nearestHit.direction);
    }

    private RaycastHit? GetClosestHit(Vector3 desiredDirection, GameObject currentCollider)
    {
        var currentPosition = transform.position;

        var origin = currentPosition + desiredDirection.normalized * collisionCastDistance;
        var maxDistance = (desiredDirection.normalized * collisionCastDistance).magnitude - radius;

        var hits = Physics.SphereCastAll(origin, radius, -desiredDirection.normalized, maxDistance, _forceSystem.GroundLayer).ToList();
        
        if (hits.Count == 0) return null;

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            if (hits[i].collider.gameObject == currentCollider.gameObject) continue;

            hits.RemoveAt(i);
        }

        var distances = hits.Select(hit => hit.distance).ToList();
        var index = distances.IndexOf(distances.Max());

        return hits[index];
    }

    private void ApplyModifier(Force targetForce)
    {
        if (targetForce.Modifier == ForceModifiers.None) return;

        _modifiers[targetForce.Modifier].Modify(targetForce, this);
    }

    private IForceable ConvertToForcetween(Force newForce)
    {
        return new ForceTween(newForce);
    }

    private void ConvertToFrameStepForce(Force targetForce)
    {
        targetForce.Direction *= Time.fixedDeltaTime;
    }

    private void DeactivateForces()
    {
        PausedForceables.AddRange(ActiveForceables);
        ActiveForceables.Clear();
    }

    public void ConcatCurrentForceables()
    {
        var oldRef = CurrentForceables;
        CurrentForceables = new List<IForceable>();
        onForcesCleared?.Invoke(new []{oldRef});
        
        CurrentForceables.AddRange(ActiveForceables);
        onForcesConcatenated?.Invoke(CurrentForceables, ActiveForceables);
        
        if (hasGravity)
        {
            CurrentForceables.AddRange(GravityForceables);
            onForcesConcatenated?.Invoke(CurrentForceables, GravityForceables);
        }
    }

    private void CallbackList(List<IForceable> targetList, Action<CallbackConfig> CallBack, bool autoRemove)
    {
        for (var i = 0; i < targetList.Count; i++)
        {
            Callback(callbacks, targetList[i],CallBack,autoRemove);
        }
    }

    public void ClearForces()
    {
        CallbackList(PausedForceables, config => config.OnCancel?.Invoke(), true);
        CallbackList(ActiveForceables, config => config.OnCancel?.Invoke(), true);
        var oldPausedRef = PausedForceables;
        var oldActiveRef = ActiveForceables;
        PausedForceables = new List<IForceable>();
        ActiveForceables = new List<IForceable>();
        ResetGravity();
        _hasCancellingForce = false;
        _hasPausingForce = false;
        hasGravity = true;
        
        onForcesCleared?.Invoke(new []{oldPausedRef, oldActiveRef});
    }

    private List<IForceable> GetTargetCollection(Force targetForce)
    {
        if (targetForce.Type == ForceTypes.Gravity) return GravityForceables;
        if (_hasPausingForce) return PausedForceables;
        return ActiveForceables;
    }

    void UpdateState(List<IForceable> targetCollection = null)
    {
        targetCollection = targetCollection == null ? ActiveForceables : targetCollection;
        var l = targetCollection.Count;
        hasGravity = true;
        _hasCancellingForce = false;
        _hasPausingForce = false;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = targetCollection[i];
            hasGravity = !currentForceable.DisablesGravity && hasGravity;
            _hasCancellingForce = currentForceable.BlendType == BlendTypes.RemoveOthers || _hasCancellingForce;
            _hasPausingForce = currentForceable.BlendType == BlendTypes.PauseOthers || _hasPausingForce;
        }
    }

    public void Callback(Dictionary<IForceable, CallbackConfig> callBacks, IForceable key, Action<CallbackConfig> Callback,
        bool autoRemove = false)
    {
        if (!callBacks.ContainsKey(key))
        {
            return;
        }

        Callback(callBacks[key]);

        if (autoRemove)
        {
            callBacks.Remove(key);
        }

    }

    public void EditCallback(Dictionary<IForceable, CallbackConfig> callBacks, IForceable key, Action<CallbackConfig> callback)
    {
        if (!callBacks.ContainsKey(key))
        {
            return;
        }

        callback(callBacks[key]);

    }

    public bool CheckCollision(Vector3 direction, float distance)
    {
        return _forceSystem.CheckCollision(this, direction, distance, CollisionLayer);
    }

    public bool CheckFrontalCollision(Vector3 direction, float distance, float threshold)
    {
        return _forceSystem.CheckFrontalCollision(this, direction, distance, CollisionLayer, threshold);
    }

    private void UnPause()
    {
        var l = PausedForceables.Count;
        var shouldBreak = false;
        for (int i = 0; i < l; i++)
        {
            var targetForce = PausedForceables[i];
            if (targetForce.BlendType == BlendTypes.RemoveOthers)
            {
                ClearForces();
                shouldBreak = true;
            }
            else if (targetForce.BlendType == BlendTypes.PauseOthers)
            {
                DeactivateForces();
                shouldBreak = true;
            }

            ActiveForceables.Add(targetForce);
            if (shouldBreak) break;
        }

        UpdateState();
    }

    public void ResetGravity()
    {
        var l = GravityForceables.Count;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = GravityForceables[i];
            currentForceable.Direction = Vector3.zero;
        }
    }

    private bool HasActiveForceables()
    {
        return ActiveForceables.Count > 0;
    }

    #region Remove


    public void Remove(IForceable targetForceable)
    {
        Remove(ActiveForceables, targetForceable);
        Remove(PausedForceables, targetForceable);
        Remove(GravityForceables, targetForceable);
    }

    private void Remove(List<IForceable> targetCollection, IForceable targetForceable)
    {
        var l = targetCollection.Count;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = targetCollection[i];
            if (currentForceable != targetForceable)
            {
                continue;
            }

            Remove(targetCollection, i);
        }
    }

    public void Remove(ForceIds forceId)
    {
        Remove(ActiveForceables, forceId);
        Remove(PausedForceables, forceId);
        Remove(GravityForceables, forceId);

    }

    void Remove(List<IForceable> targetCollection, ForceIds forceId)
    {
        var l = targetCollection.Count;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = targetCollection[i];
            if (currentForceable.Id != forceId)
            {
                continue;
            }

            Remove(targetCollection, i);
        }
    }

    public void Remove(List<IForceable> targetCollection, int index)
    {
        var targetForce = targetCollection[index];
        if (callbacks.TryGetValue(targetForce, out CallbackConfig config))
        {
            config.OnCancel?.Invoke();
        }

        targetCollection.RemoveAt(index);
        if (targetCollection == ActiveForceables)
        {
            if (!HasActiveForceables())
            {
                UnPause();
            }

            UpdateState();
        }

        ConcatCurrentForceables();
        onForceRemoved?.Invoke(targetForce, targetCollection);
    }
    void Remove(List<IForceable> targetCollection, ForceTypes forceType)
    {
        var l = targetCollection.Count;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = targetCollection[i];
            if (currentForceable.Type != forceType)
            {
                continue;
            }

            Remove(targetCollection, i);
        }
    }

    public void Remove(ForceTypes forceType)
    {
        if (forceType == ForceTypes.Gravity)
        {
            Remove(GravityForceables, forceType);
            return;
        }

        Remove(ActiveForceables, forceType);
        Remove(PausedForceables, forceType);
    }

    public void Remove(ForcePriority forcePriority)
    {
        Remove(ActiveForceables, forcePriority);
        Remove(PausedForceables, forcePriority);
        Remove(GravityForceables, forcePriority);
    }
    private void Remove(List<IForceable> targetCollection, ForcePriority forcePriority)
    {
        var l = targetCollection.Count;
        for (int i = l - 1; i >= 0; i--)
        {
            var currentForceable = targetCollection[i];
            if (currentForceable.Priority != forcePriority) continue;

            Remove(targetCollection, i);
        }
    }
    #endregion

    IForceable GetForce(ForceIds forceId)
    {
        return PausedForceables.Find(forceable => forceable.Id == forceId);
    }

    IForceable GetForce(ForceTypes forceType)
    {
        return PausedForceables.Find(forceable => forceable.Type == forceType);
    }

    List<IForceable> GetAllForces(ForceIds forceId)
    {
        return PausedForceables.FindAll(forceable => forceable.Id == forceId);
    }

    List<IForceable> GetAllForces(ForceTypes forceType)
    {
        return PausedForceables.FindAll(forceable => forceable.Type == forceType);
    }

    bool HasForce(ForceIds forceId)
    {
        return GetForce(forceId) != null;
    }

    bool HasForce(ForceTypes forceType)
    {
        return GetForce(forceType) != null;
    }

    public bool HasActiveForce(ForceIds forceId)
    {
        return HasForce(ActiveForceables, forceId);
    }

    public bool HasActiveForce(ForceIds[] forceId)
    {
        return forceId.All(id => HasForce(ActiveForceables, id));
    }

    public bool HasForce(List<IForceable> collection, ForceIds forceId)
    {
        return collection.Any(force => force.Id == forceId);
    }

    public void ResetVelocity()
    {
        Velocity = Vector3.zero;
        DesiredVelocity = Vector3.zero;
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (!value)
            {
                ResetVelocity();
            }

            _isEnabled = value;
        }
    }

    public Vector3 Velocity { get; set; }

    public Vector3 DesiredVelocity { get; set; }
}
