using System;
using UnityEngine;

public static class MathUtils
{
    
    /// <summary>
    /// Clamp a value between zero and max
    /// Difference with Mathf.Clamp is that a value above or equel to max is returned as zero
    /// </summary>
    /// <param name="value"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int RecursiveClamp(int value, int max)
    {
        return value >= max ? 0 : value;
    }
    /// <summary>
    /// Perform a Sign function, but round the zero option to 1 (default) or -1
    /// </summary>
    /// <param name="value"></param>
    /// <param name="zeroFallback"></param>
    /// <returns></returns>
    public static int SignDirection(int value, int zeroFallback = 1)
    {
        var direction = Math.Sign(value);
        return direction == 0 ? zeroFallback : direction;
    }
    
    public static int SignDirection(float value, int zeroFallback = 1)
    {
        return SignDirection((int)value, zeroFallback);
    }

}
