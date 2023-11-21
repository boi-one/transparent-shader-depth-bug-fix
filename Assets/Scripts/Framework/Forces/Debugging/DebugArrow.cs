using UnityEngine;

public abstract class DebugArrow : MonoBehaviour
{
    public abstract Color Color { get; set; }
    public abstract Vector2 Direction { get; }
    public abstract ForceDebugInfo DebugInfo { get; set; }
    public ForceBodyDebugInfo BodyDebugInfo => DebugInfo.BodyDebugInfo;
    public IForceable Force => DebugInfo.Force;
    public float ScaleModifier { get; set; } = 1f;
    public Vector3 Offset { get; set; }
}