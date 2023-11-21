using UnityEngine;

public class Stack : Modifier
{
    public override void Modify(Force targetForce, ForceBody targetForceBody)
    {
        targetForce.Direction = IsCurvedForce(targetForce)
            ? GetCurvedDirection(targetForce, targetForceBody)
            : GetDirectionPerSecond(targetForce, targetForceBody);
    }

    private Vector3 GetDirectionPerSecond(Force targetForce, ForceBody targetForceBody)
    {
        return (targetForceBody.DesiredVelocity.magnitude * targetForce.Direction) / Time.fixedDeltaTime;
    }

    private bool IsCurvedForce(Force targetForce)
    {
        return targetForce.Type == ForceTypes.Default;
    }

    private Vector3 GetCurvedDirection(Force targetForce, ForceBody targetForceBody)
    {
        var timeInterpolation = Time.fixedDeltaTime / targetForce.Duration * targetForce.Curve.GetDuration();
        var firstFrameSurface = targetForce.Curve.GetSurface(0, timeInterpolation, Force.DefaultSurfaceInterpolation);
        return targetForce.CurveSurface / firstFrameSurface * targetForceBody.DesiredVelocity.magnitude *
               targetForce.Direction;
    }
}