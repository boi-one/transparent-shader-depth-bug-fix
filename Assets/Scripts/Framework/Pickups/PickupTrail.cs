using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTrail : MonoBehaviour
{
    public PickupEvent pickupEvent;
    [SerializeField] private float trailSpeed;
    private bool _isOnLocation;
    private PickupActivator _pickupActivator;
    
    
    private void Start()
    {
        _pickupActivator = FindObjectOfType<PickupActivator>();
    }

    public void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, pickupEvent.nextPickup.transform.position,
            Vector3.Distance(pickupEvent.nextPickup.transform.position, pickupEvent.targetPickup.transform.position) * Time.deltaTime * trailSpeed);
        if (transform.position == pickupEvent.nextPickup.transform.position && !_isOnLocation)
        {
            _pickupActivator.WhenArrive(pickupEvent.nextPickup);
            _isOnLocation = true;
        }
    }
    
}
