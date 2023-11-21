using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshSettings : MonoBehaviour
{
    [SerializeField] private int sortingLayer;
    
    private void OnValidate()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = sortingLayer;
    }
}
