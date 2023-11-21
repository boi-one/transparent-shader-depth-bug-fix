using UnityEngine;

[System.Serializable]
public class Force : IForceable
{
    [SerializeField] private ForceIds id = ForceIds.Global;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 0f;
    [SerializeField] AnimationCurve curve = AnimationCurve.Linear(1, 1, 1, 1);
    [SerializeField] private BlendTypes blendType = BlendTypes.Blend;
    [SerializeField] private ForceTypes type = ForceTypes.Default;
    [SerializeField] private ForceModifiers modifier = ForceModifiers.None;
    [SerializeField] private bool isUniqueId = false;
    [SerializeField] private bool disablesGravity = false;
    [SerializeField] private bool ignoresResidualForce = false;
    [SerializeField] private ForcePriority priority = ForcePriority.High;

    public static float DefaultSurfaceInterpolation = 0.0001f;
    public float CurveSurface { get; set; }

    [Hide]
    public Vector3 CurrentForce => direction;
    
    [Hide]
    public float Interpolation => 0f;

    public Force()
    {
        UpdateSurface();
    }
    public void UpdateSurface()
    {
        CurveSurface = curve.GetSurface(DefaultSurfaceInterpolation);
    }

    public ForceIds Id
    {
        get => id;
        set => id = value;
    }

    public Vector3 Direction
    {
        get => direction;
        set => direction = value;
    }

    public float Duration
    {
        get => duration;
        set => duration = value;
    }

    public float Delay
    {
        get => delay;
        set => delay = value;
    }

    public AnimationCurve Curve
    {
        get => curve;
        set
        {
            UpdateSurface();
            curve = value;
        }
    }

    public BlendTypes BlendType
    {
        get => blendType;
        set => blendType = value;
    }

    public ForceTypes Type
    {
        get => type;
        set => type = value;
    }
    
    public ForceModifiers Modifier
    {
        get => modifier;
        set => modifier = value;
    }

    public bool IsUniqueId
    {
        get => isUniqueId;
        set => isUniqueId = value;
    }

    public bool DisablesGravity
    {
        get => disablesGravity;
        set => disablesGravity = value;
    }
    
    public bool IgnoresResidualForce
    {
        get => ignoresResidualForce;
        set => ignoresResidualForce = value;
    }

    public ForcePriority Priority
    {
        get => priority;
        set => priority = value;
    }
    
    [Hide]
    public Vector3 ResidualForce => Vector3.zero;
    
    public void Update(float deltaTime)
    {
    }

    public void Finish()
    {
    }
    
    public Force Clone()
    {
        return new Force()
        {
            id = this.id,
            direction = this.direction,
            duration = this.duration,
            delay = this.delay,
            curve = this.curve,
            blendType = this.blendType,
            type = this.type,
            modifier = this.modifier,
            isUniqueId = this.isUniqueId,
            disablesGravity = this.disablesGravity,
            priority = this.priority
        };
    }

}