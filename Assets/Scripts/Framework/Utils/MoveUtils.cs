using UnityEngine;

public static class MoveUtils
{

    public static float GetDirection(float value)
    {
        return Mathf.Sign(value);
    }
    
    public static float GetDirection(Vector2 value)
    {
        return GetDirection(value.x);
    }

    public static float GetDirection(GameObject targetGameObject)
    {
        return GetDirection(targetGameObject.transform.localScale);
    }
    
    public static float GetDirection(Transform targetTransform)
    {
        return GetDirection(targetTransform.localScale);
    }

}
