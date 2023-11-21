using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DroppingPlatform : MonoBehaviour
{
    [SerializeField] private float currentBreakTime;
    [SerializeField] private float respawnTime;
    [SerializeField] private GameObject targetPlatform;

    private float _breakDuration;
    private float _objectsOnPlatform;

    [SerializeField] private UnityEvent onObjectEnter;
    [SerializeField] private UnityEvent onObjectLeave; 
    [SerializeField] private UnityEvent onDrop;
    
    private bool _isBreaking = false;
    private bool _isDropped = false;
    
    [SerializeField] public PlatformDropTypes dropType = PlatformDropTypes.BreakAfterTime;
    public PlatformDropTypes Id
   {
       get => dropType;
       set => dropType = value;
   }

    void Start()
    {
        _breakDuration = currentBreakTime;
    }

    void Update()
    {
        if (_isDropped == true) {return; }
        CheckDropState();
        UpdateBreaking();
    }
    private void UpdateBreaking()
    {
        if(!_isBreaking){return;}
        if (!(_objectsOnPlatform > 0) && dropType != PlatformDropTypes.BreakAfterCollision) return;
        var multiplier = Mathf.Max(1, _objectsOnPlatform);
        currentBreakTime -= Time.deltaTime * multiplier;
    }
    public void OnPLatform()
    {
        _isBreaking = true;
        _objectsOnPlatform++;
        onObjectEnter?.Invoke();
    }
    public void LeavesPlatform()
    {
        _objectsOnPlatform--;
        onObjectLeave.Invoke();
    }
    private void CheckDropState()
    {
        if (currentBreakTime > 0) {return;}
        Drop();
    }
    private void Drop()
    {
        _isDropped = true;
        _isBreaking = false;
        targetPlatform.SetActive(false);
        
        Invoke ("RespawnPlatform", respawnTime);
        _objectsOnPlatform = 0;
        onDrop.Invoke();
    }
    private void RespawnPlatform()
    {
        currentBreakTime = _breakDuration;
        targetPlatform.SetActive(true);
        _isDropped = false;
    }
}