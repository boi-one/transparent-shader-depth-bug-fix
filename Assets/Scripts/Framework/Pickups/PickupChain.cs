using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
//pickups[i].parentChain = this;
[Serializable]
public class PickupChain
{
    [SerializeField] private bool isOrdered;

    public UnityEvent onChainComplete = new UnityEvent();
    public UnityEvent<PickupEvent> onPickup = new UnityEvent<PickupEvent>();
    public UnityEvent<Pickup> onNextPickup = new UnityEvent<Pickup>();
    public UnityEvent<Pickup> onLastPickup = new UnityEvent<Pickup>();
    public List<Pickup> pickups = new List<Pickup>();
    private int _currentItemIndex = -1;
    
    public void Init()
    {
        var lenght = pickups.Count;
        for (int i = 0; i < lenght; i++)
        {
            pickups[i].parentChain = this;
        }
        Next();
    }
    


    public void AddPickup(Pickup targetPickup)
    {
        targetPickup.IsPickedUp = IsOrdered ? AddOrdered(targetPickup) : AddUnordered();
        
        if(targetPickup.IsPickedUp)onPickup?.Invoke(new PickupEvent()
        {
            targetPickup = targetPickup,
            nextPickup = CurrentPickup
        });
        
        if (!IsComplete) return;
        onChainComplete?.Invoke();
        onLastPickup?.Invoke(targetPickup);
    }

    private bool AddUnordered()
    {
        Next();
        return true;
    }

    private void Next()
    {
        _currentItemIndex++;
        if(CurrentPickup != null)
            onNextPickup?.Invoke(CurrentPickup);
    }

    private bool AddOrdered(Pickup pickup)
    {
        var index = pickups.IndexOf(pickup);
        if (index == _currentItemIndex)
        {
            Next();
            return true;
        }
        return false;
    }

    public bool HasPickups()
    {
        return !pickups.IsEmpty();
    }

    public Pickup CurrentPickup => _currentItemIndex < pickups.Count ? pickups[_currentItemIndex] : null;

    public bool IsComplete
    {
        get => _currentItemIndex == pickups.Count;
    }

    public bool IsOrdered
    {
        get => isOrdered;
        set => isOrdered = value;
    }
}
