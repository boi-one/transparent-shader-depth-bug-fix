using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayerJoin : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private Transform spawnpoint;

    private CinamachineTargetGroupApi _cinamachineTargetGroupApi;
    
    private void Start()
    {
        _cinamachineTargetGroupApi = GetComponent<CinamachineTargetGroupApi>();
        inputManager.playerJoinedEvent.AddListener(AddPlayerTarget);
        inputManager.playerLeftEvent.AddListener(RemovePlayerTarget);
    }
    
    private void AddPlayerTarget(PlayerInput input)
    {
        _cinamachineTargetGroupApi.RemoveTarget(spawnpoint);
        _cinamachineTargetGroupApi.RemoveTarget(spawnpoint);
        _cinamachineTargetGroupApi.AddTarget(input.transform);
    }

    public void RemovePlayerTarget(PlayerInput input)
    {
        _cinamachineTargetGroupApi.RemoveTarget(input.transform);
        _cinamachineTargetGroupApi.AddTarget(spawnpoint);
    }
}