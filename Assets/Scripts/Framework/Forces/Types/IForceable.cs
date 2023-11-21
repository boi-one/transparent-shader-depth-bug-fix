using UnityEngine;

public interface IForceable
{
    Vector3 CurrentForce { get; }
    float Interpolation { get; }
    Vector3 ResidualForce { get; }
    public Vector3 Direction { get; set; }
    public ForceTypes Type { get; set; }
    public ForceModifiers Modifier { get; set; }
    public BlendTypes BlendType { get; set; }
    public ForcePriority Priority { get; set; }
    public bool DisablesGravity { get; set; }
    public bool IgnoresResidualForce { get; set; }
    public ForceIds Id { get; set; }
    void Update(float deltaTime);
    void Finish();
}