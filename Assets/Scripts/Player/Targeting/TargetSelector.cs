using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TargetSelector : AimingSystem
{
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private LayerMask blockTargetingLayers;
    [SerializeField, Tooltip("If a object is blocking your path, how close can it be to the target before you can't pull anymore?")] private float maxCollisionOffset = 1.5f;
    [SerializeField, Range(-1, 1)] private float angleTolerance = 0.2f;
    [SerializeField] private float maxSelectableDistance = 50f;
    [SerializeField] private UnityEvent<Vector2> targetAimChanged = new UnityEvent<Vector2>();

    [SerializeField] private UnityEvent<Selectable> onDeselectTarget = new UnityEvent<Selectable>();
    
    [SerializeField] private List<Selectable> ignoredSelectables;

    [SerializeField] protected Selectable[] selectables;
    
    public Selectable[] Selectables => selectables;

    [SerializeField] private List<Selectable> selectableColliders;
    [SerializeField] private Collider[] previousHitColliders;

    private Selectable _selected;

    protected Vector2 direction;

    public UnityEvent selectNew = new UnityEvent();

    private bool _isUsable = true;

    private Color _currentColor;
    public Color CurrentColor
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            ChangeColor(_currentColor);
        }
    }

    protected Dictionary<Selectable, Renderer> _spottedSelectables = new Dictionary<Selectable, Renderer>();
    
    [SerializeField] private GameObject selectorVisual;

    private Dictionary<Collider, Selectable> _currentSelectables = new Dictionary<Collider, Selectable>();

    protected virtual void Awake()
    {
        //Origin default value
        Origin = transform;
        Setup(Origin);
    }
    
    private void Update()
    {
        if (_selected != null && !CanSelect(_selected)) Deselect();
    }

    public bool IsUsable
    {
        get => _isUsable;
        set
        {
            _isUsable = value;
            if(!_isUsable) Deselect();
        }
    }

    public virtual void Activate()
    {
        if (_selected != null && !IsSelectable(_selected)) Deselect();

        var hitColliders = Physics.OverlapSphere(transform.position, maxSelectableDistance, blockingLayers);

        var hasSameSelectables = hitColliders.Length - 1 == 0 || hitColliders.Length - 1 == selectables.Length && hitColliders.Contains(previousHitColliders);
        if (hasSameSelectables) return;
        selectableColliders.Clear();

        foreach (var hitCollider in hitColliders)
        {
            if (!_currentSelectables.ContainsKey(hitCollider)) _currentSelectables.Add(hitCollider, hitCollider.GetComponentInChildren<Selectable>());
            var currentSelectable = _currentSelectables[hitCollider];

            if (currentSelectable == null) continue;

            if(!_spottedSelectables.ContainsKey(currentSelectable)) _spottedSelectables.Add(currentSelectable,currentSelectable.GetComponent<Renderer>());

            if (ignoredSelectables.Contains(currentSelectable) || currentSelectable == null) continue;
            UpdateTargetSelectables(selectableColliders, currentSelectable);
        }

        previousHitColliders = hitColliders;
        selectables = selectableColliders.ToArray();
    }

    public void AddToIgnoredSelectables(Selectable selectable = null)
    {
        if(_selected == null && selectable == null) return;

        if (selectable == null && _selected != null)
        {
            selectable = _selected;
            Deselect();
        }
        
        ignoredSelectables.Add(selectable);
    }
    
    public void RemoveFromIgnoredSelectables(Selectable selectable = null)
    {
        if (selectable == null) selectable = _selected;
        
        ignoredSelectables.Remove(selectable);
    }


    protected virtual bool IsSelectable(Selectable selectable)
    {
        return selectable != null;
    }

    protected virtual void UpdateTargetSelectables(List<Selectable> totalList, Selectable selectable)
    {
        if (!IsSelectable(selectable))
        {
            if (totalList.Contains(selectable)) totalList.Remove(selectable);
            return;
        }
        
        totalList.Add(selectable);
    }

    public Transform Origin { get; set; }
    
    public Vector3 CurrentDirection
    {
        get
        {
            if (CanAimFreely)
            {
                //Check if target is selected
                Selectable target = GetSelected();
                if (target != null)
                {
                    //Target aim
                    return (target.GetPosition() - Origin.position).normalized;
                }

                //Free aim
                return GetDirection();
            }
        
            //Default aim
            return new Vector3(Mathf.Sign(transform.localScale.x), 0, 0);
        }
    }

    protected override void InputMove(Vector2 input)
    {
        if (!IsUsable) return;
        base.InputMove(input);
        
        direction = input.normalized;

        DetermineTarget();
        
        targetAimChanged?.Invoke(CurrentDirection);
    }

    protected virtual void DetermineTarget()
    {
        var selectable = GetValidSelectables(direction).OrderBy(aSelectable => Vector2.Dot((aSelectable.GetPosition() - transform.position).normalized, direction)).LastOrDefault(CanSelect);
        
        if (selectable != null) Select(selectable);
        else Deselect();
    }

    public Selectable GetSelected()
    {
        return _selected;
    }

    public bool CanSelect(Selectable selectable)
    {
        if (blockTargetingLayers == (blockTargetingLayers | (1 << selectable.gameObject.layer)) || !CanAimFreely) return false;
        
        Vector3 myPosition = transform.position;
        Vector2 dir =selectable.GetPosition() - myPosition;
        
        float maxDistance = Vector2.Distance(myPosition, selectable.GetPosition());
        if (maxDistance > maxSelectableDistance) return false;        

        if (Physics.Raycast(transform.position, dir, out var hit, maxDistance, blockingLayers))
        {
            if (hit.collider == null) return true;
            if (Vector2.Distance(hit.collider.transform.position, hit.point) < maxCollisionOffset) return true;
            if (hit.collider.gameObject != selectable.transform.gameObject)
            {
                return false;
            }
        }

        return true;
    }

    public void Reset()
    {
        SetParent(transform);
        selectorVisual.SetActive(false);
    }

    private void SetParent(Transform targetTransform)
    {
        selectorVisual.transform.parent = targetTransform;
    }

    public void Select(Selectable selectable)
    {
        SetParent(selectable.transform);
        if (selectable == _selected) return;
        
        selectable.AddSelector(this);

        selectorVisual.transform.position = selectable.GetPosition();

        selectNew?.Invoke();
        _selected = selectable;
        selectorVisual.SetActive(true);

    }
    

    public void Deselect()
    {
        selectorVisual.SetActive(false);
        if (_selected == null) return;

        SetParent(_selected.transform);

        selectNew?.Invoke(); 
        _selected = null;
    }

    private void OnDestroy()
    {
        Destroy(selectorVisual);
    }

    public Selectable[] GetValidSelectables(Vector2 dir)
    {
        //return _selectables;
        return selectables.Where(selectable => selectable != null && Vector2.Dot((selectable.GetPosition() - transform.position).normalized, dir) > angleTolerance).ToArray();
    }
    
    public void ChangeColor(Color color)
    {
        var spriteRenderers = selectorVisual.GetComponentsInChildren<SpriteRenderer>();
        var l = spriteRenderers.Length;

        for (int i = 0; i < l; i++)
            spriteRenderers[i].color = color;
    }


    public Vector2 GetDirection()
    {
        return direction;
    }
    
    public void Disable()
    {
        if (IsUsable) IsUsable = false;
    }

    public void Enable()
    {
        if (!IsUsable) IsUsable = true;
    }
    
    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxSelectableDistance);
    }
}