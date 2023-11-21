using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AimObjectRotater : MonoBehaviour
{
    [SerializeField] private GameObject aimObject;

    public bool CanRotate { get; set; }

    public Vector2 CurrentDirection { get; set; }

    public int CurrentLookDirection { get; set; }

    private void Update()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        if (!CanRotate) return;

        Vector2 currentDirection = CurrentDirection;
        
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        aimObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void ResetObjectRotation()
    {
        int direction = CurrentLookDirection;

        var objectRotation = direction == 1 ? 0 : 180;

        aimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, objectRotation));
    }
    
    public void FlipObject(int dir)
    {
        aimObject.transform.localScale = new Vector3(dir, 1, 1);
    }

    public void FlipRotateObject(int dir)
    {
        FlipObject(dir);
        var objectRotation = dir == 1 ? 0 : 180;
        aimObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, objectRotation));
    }
}
