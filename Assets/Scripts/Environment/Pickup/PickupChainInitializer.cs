#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PickupChainInitializer : MonoBehaviour, IInitializable
{
    private PickupSystem _pickupSystem;
    [SerializeField] private bool isOrdered;
    [SerializeField] private int amountOfObjectsTospawn = 3;
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private List<Vector3> offset;

    public void Initialize()
    {
        
        transform.parent = PickupSystem.Instance.gameObject.transform;

        var newChain = new PickupChain();
        PickupSystem.Instance.PickupChains.Add(newChain);
        newChain.IsOrdered = isOrdered;

        SpawnObjects();

        for (int i = 0; i < transform.childCount; i++)
        {
            var chainPickups = transform.GetChild(i).GetComponent<Pickup>();

            if (!chainPickups == false)
            {
                newChain.pickups.Add(chainPickups);
            }
            
        }
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < amountOfObjectsTospawn; i++)
        {
            var newObjects = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
            
            newObjects.transform.position = transform.position + offset[i];
            newObjects.transform.parent = gameObject.transform;
            
            
        }
    }
}

#endif