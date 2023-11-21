using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSystem : MonoBehaviour
{
    public List<GameObject> targetCollisions = new List<GameObject>();
    public BoxCollider[] hitBoxColliders;

    public Bounds CurrentHitBounds => hitBoxColliders[CurrentHitboxNumber].bounds;
    private bool _isPerformingAttack;
    private int _currentHitboxNumber;

    public int CurrentHitboxNumber
    {
        get => _currentHitboxNumber;
        set
        {
            _currentHitboxNumber = value;
        }
    }

    public bool HasHitATarget => targetCollisions.Count > 0;

    public bool IsPerformingAttack
    {
        get => _isPerformingAttack;
        set
        {
            if (value && !_isPerformingAttack)
            {
                targetCollisions.Clear();
            }
            _isPerformingAttack = value;
        }
    }
    public bool CheckHasHit(GameObject target)
    {
        
        return targetCollisions.Contains(target);
    }
}
