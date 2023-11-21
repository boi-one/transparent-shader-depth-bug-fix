using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo-Attack", menuName = "Combo Attack")]
public class ComboAttack : ScriptableObject
{
        [Header("Combo Settings")]
        public float damage;
        [Range(0, 10)] public float forceIntensity;
        public Vector2 forceDirection;
        public AnimationClip animation;
        public float animationSpeed;
        public bool inAir;
        public bool triggersHitState;
        public float gravityScale;
        public int currentHitboxNumber;
        [Range(0,1)] public float startActiveHitbox;
        [Range(0,1)] public float endActiveHitbox;
    
        [Header("Freeze Frame Settings")]
        public FreezeTiming freezeTiming;
        public float freezeTime;
        public bool isStateUncancellable;
        public FreezeFrameIds frameId;

        [Header("Particle Settings")] 
        public bool useParticle;
        public int particleID;
}
