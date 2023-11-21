using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointVisual : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Vector3 _playercenter;
    private Vector3 _playersize;
    
    void OnDrawGizmos()
    {
        _playercenter = player.GetComponent<BoxCollider>().center;
        _playersize = player.GetComponent<BoxCollider>().size;
        
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position + _playercenter, _playersize);
    }
}
