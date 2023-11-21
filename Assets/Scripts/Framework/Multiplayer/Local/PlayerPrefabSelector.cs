using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerPrefabSelector : MonoBehaviour
{

    [SerializeField] private List<PlayerInformation> playerPrefabQueue;
    [SerializeField] private PlayerInputManager playerInputManager;

    private int _currentCharacterIndex = -1;
    
    [System.Serializable]
    public struct PlayerInformation
    {
        public GameObject playerPrefab;
        public Color playerColor;
    }
    
    void Start()
    {
        SelectFirst();
    }

    public void SelectNext()
    {
        var newIndex = _currentCharacterIndex < playerPrefabQueue.Count - 1 ? _currentCharacterIndex + 1 : 0;
        Select(newIndex);
    }
    
    public void SelectPrevious()
    {
        var newIndex = _currentCharacterIndex > 0 ? _currentCharacterIndex - 1 : playerPrefabQueue.Count - 1;
        Select(newIndex);
    }

    public void SelectFirst()
    {
        Select(0);
    }
    
    public void SelectLast()
    {
        Select(playerPrefabQueue.Count - 1);
    }

    public void Select(int index)
    {
        var targetCharacterSettings = playerPrefabQueue[index];
        var targetCharacterPrefab = targetCharacterSettings.playerPrefab;
        playerInputManager.playerPrefab = targetCharacterPrefab;
        _currentCharacterIndex = index;

        var targetSelector = targetCharacterPrefab.GetComponent<TargetSelector>();
        if(targetSelector == null) return;

        targetSelector.CurrentColor = playerPrefabQueue[index].playerColor;
    }

}
