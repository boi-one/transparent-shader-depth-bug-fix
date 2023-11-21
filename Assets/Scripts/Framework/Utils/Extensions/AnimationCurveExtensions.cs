using UnityEngine;

public static class AnimationCurveExtensions
{
    
    public static float GetSurface(this AnimationCurve targetCurve, float interval = 0.01f)
    {
        return targetCurve.GetSurface(0, targetCurve.GetDuration());
    }
    
    public static float GetSurface(this AnimationCurve targetCurve, float start, float end, float interval = 0.01f)
    {
        var duration = end - start;
        var surface = 0f;
        var previousCurve = targetCurve.Evaluate(start);
        for (int i = 0; i < duration/interval; i++)
        {
            var currentCurve = targetCurve.Evaluate(start + interval * (i + 1));
            var avgCurve = (currentCurve + previousCurve) / 2;
            surface += avgCurve * interval;
            previousCurve = currentCurve;
        }
        return surface;
    }
    
    public static float GetDuration(this AnimationCurve targetCurve)
    {
        return 1f;
    }

    public static float GetLastKeyframeValue(this AnimationCurve targetCurve)
    {
        var lastKey = targetCurve.keys[targetCurve.keys.Length - 1];
        return lastKey.value;
    }
}
