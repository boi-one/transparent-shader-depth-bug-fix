using UnityEngine;

public class TransformBlockadeData : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] private GameObject referrerGameObject;
    [SerializeField] private bool blockPosition, blockRotation, blockScale;
    
    public string Description => description;
    public GameObject ReferrerGameObject => referrerGameObject;
    public bool BlockPosition => blockPosition;
    public bool BlockRotation => blockRotation;
    public bool BlockScale => blockScale;
}