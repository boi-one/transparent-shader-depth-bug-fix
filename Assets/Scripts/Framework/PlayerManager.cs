using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : GenericSingleton<PlayerManager>
{
    [SerializeField] private PlayerInputManager playerInputManager;
    private List<GameObject> _playersInGame = new List<GameObject>();

    public List<GameObject> PlayersInGame
    {
        get => _playersInGame;
        set => _playersInGame = value;
    }

    public void Awake()
    {
        playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        ReplaceName(playerInput);
        PlayersInGame.Add(playerInput.gameObject);
    }

    public void ReplaceName(PlayerInput playerInput)
    {
        playerInput.name = playerInput.name.Replace("(Clone)", "");
    }

    public int GetCharacterCount(Character characterType)
    {
        return _playersInGame.Count(playerObj => playerObj.name == characterType.ToString());
    }

    public void ClearPlayers()
    {
        _playersInGame.Clear();
    }
}