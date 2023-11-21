using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(PlayerInputManager))]
public class PlayerJoinHandler : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    private void Awake()
    {
        var playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoin);
        playerInputManager.playerLeftEvent.AddListener(OnPlayerLeave);
    }

    private void OnPlayerJoin(PlayerInput input)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning($"{spawnPoint} is empty");
        }
        input.transform.position = spawnPoint.position;

        PlayerMovement playerMovement = input.gameObject.GetComponent<PlayerMovement>();
        SceneManager.MoveGameObjectToScene(input.gameObject, SceneManager.GetSceneAt(1));

    }

    private void OnPlayerLeave(PlayerInput input)
    {
        
    }
}
