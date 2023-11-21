using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    private PickUpTypes _pickUpType = PickUpTypes.Disable;

    [SerializeField] private int pickupDuration;
    [SerializeField] private UnityEvent onPickup = new UnityEvent();
    
    [HideInInspector] [CanBeNull] public PickupChain parentChain;

    private bool _isPickedUp;
    public bool IsPickedUp
    {
        get => _isPickedUp;
        set
        {
            _isPickedUp = value;
            SetActivation(_isPickedUp);
        }
    }
    public PickUpTypes PickUpType
    {
        get => _pickUpType;
        set => _pickUpType = value;
    }

    public void PickUp() => PickupSystem.Instance.AddPickup(this);

    private void SetActivation(bool isPickedUp) => HandlePickupActivation(isPickedUp);

    public bool HasChain()
    {
        return parentChain.HasPickups();
    }

    private void HandlePickupActivation(bool isPickedUp)
    {
        if (_pickUpType == PickUpTypes.DoNothing) return;
        if (!isPickedUp) return;
        onPickup?.Invoke();
        Invoke("Deactivate", pickupDuration);
    }
    private void Deactivate()
    {
        if (_pickUpType == PickUpTypes.Destroy)
        {
            Destroy(visual);
        }
        if (_pickUpType == PickUpTypes.Disable)
        {
            visual.SetActive(false);
        }
    }

}
