 using System;
using System.Collections.Generic;
 using System.Linq;
 using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
 using UnityEngine.Rendering;
 using UnityEngine.Serialization;

public class PushAbility : Ability
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private UnityEvent<PushData> onPushEventNotGrounded = new UnityEvent<PushData>();
    [SerializeField] private UnityEvent<PushData> onPushEventGrounded = new UnityEvent<PushData>();
    [SerializeField] private UnityEvent onPushComplete = new UnityEvent();

    [SerializeField] private float distanceForce = 10f;

    [SerializeField] private Transform pushOrigin;
    
    [SerializeField] private Force pushForce;

    [SerializeField] private bool isAtractMode = false;

    private  bool isPushed = false;

    private void Awake() 
    {
        OnCanUseAbilityChange.AddListener((newValue) =>
        {
            if (newValue) onPushComplete?.Invoke();
        });
    }

    private void Start()
    { 
       pushOrigin ??= transform;
       Init();
   }

   public override bool Use(InputAction.CallbackContext context)
   {
       return Use();
   }
   public void Trigger()
   { 
       Use();
   }

   public bool Use()
   {
       if (!CanUseAbility) return false;
       isPushed = false;
       List<GameObject> pushedObjects = new List<GameObject>();
        EmitUse();
        foreach (GameObject currentTarget in GameObjectsInRange())
        {
            if (!PushObject(currentTarget)) continue;
            pushedObjects.Add(currentTarget);
        }
        
        onPushEventGrounded?.Invoke(new PushData(pushOrigin.position, pushedObjects));

        CanUseAbility = false;
        StartDelay();
        return true;
   }

    /// <summary>
    /// Pushes objects in a certain direction.
    /// </summary>
    private bool PushObject(GameObject target)
    {
        if (target.HasComponent<Pushable>())
        {
            Pushable pushable = target.GetComponent<Pushable>();
            pushable.Impact();
        }

        if (target.HasComponent<ForceBody>())
        {
            var forceBody = target.GetComponent<ForceBody>();
            var force = GetForce(target, forceBody.Mass);
            
            if (isAtractMode)
            {
                force = GetForce(target, forceBody.Mass * -1);
            }
            
            forceBody.Add(force);
        }
        
        return true;
    }
    
    /// <summary>
    /// The distanceForce is determined by how far the target object is in perspective of the object
    /// this script is attached to and the weight of the target object.
    /// </summary>
    private Force GetForce(GameObject target, float mass = 1)
    {
        var targetForce = pushForce.Clone();
        var direction = target.transform.position - pushOrigin.position;
        var closestPoint = target.GetComponent<Collider>().ClosestPoint(pushOrigin.position);
        var hitDistance = (closestPoint - pushOrigin.position).magnitude;
        float impact = 1;//Mathf.Max((radius - hitDistance) / radius, 0);
        var impactForce = direction.normalized * impact * this.distanceForce;
        targetForce.Direction = impactForce / mass;
        return targetForce;
    }

    /// <summary>
    /// Returns the radius of the sphere.
    /// </summary>
    public float GetRadius()
    {
        return radius;
    }

    /// <summary>
    /// Returns list of GameObjects.
    /// The radius is determined by the radius variable.
    /// Origin position is the position of the game object this script is attached to.
    /// Ignors game object this script is attached to.
    /// </summary>
    private List<GameObject> GameObjectsInRange()
    {
        List<GameObject> targets = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(pushOrigin.position, radius);

        foreach (Collider collider in hitColliders)
        {
            if (!isColliderValid(collider, targets)) continue;

            targets.Add(collider.gameObject);
        }

        return targets;
    }

    private bool isColliderValid(Collider collider, List<GameObject> targets)
    {
        return collider != null && collider.gameObject != null && !collider.gameObject.Equals(gameObject) &&
               !targets.Contains(collider.gameObject);
    } 
    
    public override void OnInput(InputAction.CallbackContext aContext)
    {
        Use(aContext);
    }
}