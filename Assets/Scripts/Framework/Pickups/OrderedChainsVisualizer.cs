using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class OrderedChainsVisualizer : MonoBehaviour
{

    [SerializeField] private PickupSystem pickupSystem;
    [SerializeField] private GameObject chainVisual;
    [SerializeField] private string nameOfPuller;
    private Dictionary<Pickup, GameObject> _chainConnections = new Dictionary<Pickup, GameObject>();

    public PickupSystem PickupSystem
    {
        get => PickupSystem.Instance;
    }

    void Start()
    {
        chainSetup();
    }

    private void chainSetup()
    {
        foreach (PickupChain chain in PickupSystem.PickupChains)
        {
            if(!chain.IsOrdered)continue;
            
            chain.onPickup.AddListener(onPickup);
            AddToDictionary(chain);

        }
    }

    private void onPickup(PickupEvent pickupEvent)
    {
        if (!_chainConnections.ContainsKey(pickupEvent.targetPickup)) return;
        Destroy(_chainConnections[pickupEvent.targetPickup]);
        _chainConnections.Remove(pickupEvent.targetPickup);
    }

    private void AddToDictionary(PickupChain chain)
    {
        for (int i = 0; i < chain.pickups.Count - 1; i++)
        {
            GameObject currentVisual = Instantiate(chainVisual, chain.pickups[i].transform);
                
            _chainConnections[chain.pickups[i]] = currentVisual;
                
            currentVisual.transform.Find(nameOfPuller).transform.position = chain.pickups[i + 1].transform.position;
        }
    }
}
