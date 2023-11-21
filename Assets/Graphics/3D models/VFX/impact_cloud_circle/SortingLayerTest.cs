using UnityEngine;
public class SortingLayerTest : MonoBehaviour
{
    public int sortingLayer;
    private void Awake()
    {
        GetComponent<MeshRenderer>().sortingOrder = sortingLayer;
    }
}