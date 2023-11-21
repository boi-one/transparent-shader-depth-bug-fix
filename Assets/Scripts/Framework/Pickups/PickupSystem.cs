using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PickupSystem : GenericSingleton<PickupSystem>
{
    private bool _allChainsComplete;
    private bool _everythingIsCollected;

    [Header("Events")]
    [SerializeField] private UnityEvent onEverythingComplete = new UnityEvent();
    [SerializeField] private UnityEvent onAllSeparatePickupsCollected = new UnityEvent();
    [SerializeField] private UnityEvent onAllChainsComplete = new UnityEvent();
    
    [SerializeField] private UnityEvent<PickupEvent> onPickUp = new UnityEvent<PickupEvent>();
    [SerializeField] private UnityEvent<Pickup> isFinalPickUp = new UnityEvent<Pickup>();
    [SerializeField] private UnityEvent<Pickup> onNextPickup = new UnityEvent<Pickup>();
    [SerializeField] private UnityEvent<PickupEvent> OnPickupEvent = new UnityEvent<PickupEvent>();
    
    [Header("Lists")]
    [SerializeField] private List<Pickup> allInteractiveItems;
    [SerializeField] private List<Pickup> separateInteractiveItems;
    [SerializeField] private List<PickupChain> pickupChains = new List<PickupChain>();

    private PickupEvent _currentPickupEvent;
    public List<PickupChain> PickupChains
    {
        get => pickupChains;
        set => pickupChains = value;
    }

    private void Start()
    {
        InitializeChains();
        FindAllPickups(); 
    }

    private void InitializeChains()
    {
        var length = PickupChains.Count;
        if (PickupChains.IsEmpty()) return;
        for (int i = 0; i < length; i++)
        {
            PickupChains[i].onChainComplete.AddListener(OnChainCompleted); 
            PickupChains[i].onNextPickup.AddListener(OnNextPickup); 
            PickupChains[i].onPickup.AddListener(OnPickup); 
            PickupChains[i].onLastPickup.AddListener(IsFinalPickUp);
            PickupChains[i].Init();
        }
    }

    private void OnNextPickup(Pickup currentPickup)
    {
        onNextPickup?.Invoke(currentPickup);
    }
    
    private void OnPickup(PickupEvent pickupEvent)
    {
        onPickUp?.Invoke(pickupEvent);
        OnPickupEvent?.Invoke(_currentPickupEvent);   
        
    }
    
    private void IsFinalPickUp(Pickup currentPickup)
    {
        isFinalPickUp?.Invoke(currentPickup);
    }

    private void OnChainCompleted()
    {
        CheckAllCollected();
        CheckAllChainsDone();
    }

    private void FindAllPickups()
    {
        var allInteractivePickups = FindObjectsOfType<Pickup>();
        var length = allInteractivePickups.Length;
        for (int i = 0; i < length; i++)
        {
            var currentPickup = allInteractivePickups[i];
            allInteractiveItems.Add(currentPickup);
            if (!currentPickup.HasChain()) separateInteractiveItems.Add(currentPickup);
        }
    }

    public void AddPickup(Pickup targetPickup)
    {
        if(targetPickup.IsPickedUp) return;

        if (targetPickup.parentChain.HasPickups())
        {
            targetPickup.parentChain.AddPickup(targetPickup);
            return;
        }
        
        targetPickup.IsPickedUp = true;
        if (!separateInteractiveItems.All(item => item.IsPickedUp)) return;
        onAllSeparatePickupsCollected?.Invoke();
        CheckAllCollected();
    }

    private void CheckAllChainsDone()
    {
        if (!PickupChains.All(chain => chain.IsComplete)) return;
        
        _allChainsComplete = true;
        onAllChainsComplete?.Invoke();
    }

    private void CheckAllCollected()
    {
        if (!_everythingIsCollected && _allChainsComplete && allInteractiveItems.All(item => item.IsPickedUp))
        {
            _everythingIsCollected = true;
            onEverythingComplete?.Invoke();
        }
    }
}


