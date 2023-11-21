using UnityEngine;

public class LaserVisualizer : MonoBehaviour
{
    private const float ScaleOffset = 0.75f;
    private const float YScale = 0.75f;
    
    [SerializeField] private GameObject targetVisual;
    [SerializeField] private GameObject targetEndPoint;
    [SerializeField] private GameObject laserEndCap;

    private void ScaleTargetVisual(float distance)
    {
        var scaleChange = new Vector3(distance / 2 - ScaleOffset, YScale, 1);
        targetVisual.transform.localScale = scaleChange;
    }

    private void UpdateEndPoint(float distance, Vector3 endPosition)
    {
        targetEndPoint.transform.position = endPosition;
        var endCapPlacementOffset = 1.5f;
        var localEndCapPosition = new Vector3(distance - endCapPlacementOffset, 0);
        laserEndCap.transform.localPosition = localEndCapPosition;
    }

    public void Reset()
    {
        targetEndPoint.transform.position = gameObject.transform.position;
    }

    public void OnLaserUpdate(Vector3 startPosition, Vector3 endPosition, RaycastHit hit)
    {
        var distance = (endPosition - startPosition).magnitude;
        UpdateEndPoint(distance, endPosition);
        ScaleTargetVisual(distance);
        RotateHitVisual(hit.normal);
    }

    private void RotateHitVisual(Vector3 hitNormal)
    {
        targetEndPoint.transform.RotateToDirection(hitNormal, 180);
    }
}