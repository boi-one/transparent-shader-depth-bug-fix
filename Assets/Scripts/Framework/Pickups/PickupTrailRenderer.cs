using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PickupTrailRenderer : MonoBehaviour
{
    [SerializeField] private GameObject trailPrefab;
  
    private float _despawndelay = 1f;

    public void Render(PickupEvent pickupEvent)
    {
        if (pickupEvent.nextPickup == null)
        {
            return;
        }

        GameObject currentTrail = Instantiate(trailPrefab, pickupEvent.targetPickup.transform.position, Quaternion.identity);
        currentTrail.GetComponent<PickupTrail>().pickupEvent = pickupEvent;
        Destroy(currentTrail, _despawndelay);
    }
}
