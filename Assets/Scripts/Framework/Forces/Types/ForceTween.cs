using UnityEngine;

public class ForceTween : IForceable
{
    [SerializeField] private Force force;
    [SerializeField] private float currentTime = 0;
    private float _lastInterpolation = 0;
    private Vector3 _currentForce;
    private float interval = 0.0001f;

    private Vector3 _appliedForce = new Vector3();

    public ForceTween(Force force)
    {
        this.force = force;
    }

    public void Update(float deltaTime)
    {
        currentTime += deltaTime;
        _currentForce = CalculateCurrentForce();
    }

    public void Finish()
    {
        currentTime = force.Duration + force.Delay;
        _currentForce = force.Direction - _appliedForce;
    }

    /// <summary>
    /// Return the progress from 0 to 1
    /// </summary>
    public float Interpolation => Mathf.Min(1f, Mathf.Max(currentTime - force.Delay, 0) / force.Duration);

    private Vector3 CalculateCurrentForce()
    {
        var deltaT = Interpolation - _lastInterpolation;

        var steps = Mathf.Ceil(deltaT / interval);
        // todo: performance updaten van GetSurface
        var deltaSurface = force.Curve.GetSurface(_lastInterpolation, _lastInterpolation + steps * interval, interval);
        var forceMagnitude = deltaSurface / force.CurveSurface;

        _lastInterpolation += steps * interval;

        var desiredForce = force.Direction * forceMagnitude;
        _appliedForce += desiredForce;
        return desiredForce;
    }

    public void Reset()
    {
        currentTime = 0;
        _lastInterpolation = 0;
        _appliedForce = new Vector3();
    }

    public Vector3 ResidualForce
    {
        get
        {
            if (force.Curve.GetLastKeyframeValue() <= 0) return Vector3.zero;
            
            var duration = force.Curve.GetDuration();
            var samplingPerentage = 0.01f;
            var residualSurface = force.Curve.GetSurface(duration - samplingPerentage, duration) / samplingPerentage;
            var residualMagnitude = residualSurface / force.CurveSurface;
            return (force.Direction * residualMagnitude) / force.Duration;
        }
    }

    public Vector3 CurrentForce => _currentForce;

    public Vector3 Direction
    {
        get => force.Direction;
        set => force.Direction = value;
    }

    public ForceTypes Type
    {
        get => force.Type;
        set => force.Type = value;
    }
    
    public ForceModifiers Modifier
    {
        get => force.Modifier;
        set => force.Modifier = value;
    }
    
    public BlendTypes BlendType
    {
        get => force.BlendType;
        set => force.BlendType = value;
    }

    public ForcePriority Priority
    {
        get => force.Priority;
        set => force.Priority = value;
    }

    public bool DisablesGravity
    {
        get => force.DisablesGravity;
        set => force.DisablesGravity = value;
    }
    
    public bool IgnoresResidualForce
    {
        get => force.IgnoresResidualForce;
        set => force.IgnoresResidualForce = value;
    }
    
    public ForceIds Id
    {
        get => force.Id;
        set => force.Id = value;
    }

    public Force Force
    {
        get => force;
    }
}