using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AimingSystem : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float deadZone = 0.41f;
    [SerializeField] private UnityEvent<Vector2> aimChanged = new UnityEvent<Vector2>();
    [SerializeField] private UnityEvent freeAimEnabled = new UnityEvent();
    [SerializeField] private UnityEvent freeAimDisabled = new UnityEvent();
    private Transform _centerPoint;
    private bool _canAimFreely = false;

    private void Awake()
    {
        Setup(transform);
    }

    public bool CanAimFreely
    {
        get => _canAimFreely;
        set
        {
            _canAimFreely = value;
            if(_canAimFreely) freeAimEnabled?.Invoke();
            else freeAimDisabled?.Invoke();
        }
    }

    public void Setup(Transform centerPoint)
    {
        _centerPoint = centerPoint;
    }

    public void MouseMove(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();

        Vector2 diffVector = ScreenUtilities.GetWorldDirection(new Vector3(mousePos.x, mousePos.y, 100), _centerPoint.position);

        InputMove(diffVector);
    }

    public void ControllerMove(InputAction.CallbackContext context)
    {
        Vector2 controllerInput = context.ReadValue<Vector2>();
        if(controllerInput.magnitude < deadZone) return;
        InputMove(controllerInput);
    }

    protected virtual void InputMove(Vector2 direction)
    {
        aimChanged?.Invoke(direction);
    }
}