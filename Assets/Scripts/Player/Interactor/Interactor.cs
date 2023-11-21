using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    public UnityEvent<GameObject> onObjectEntered = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> onObjectExit = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> onObjectStayed = new UnityEvent<GameObject>();

    [SerializeField] private bool includeSelf = false;
    [SerializeField] private Vector3 halfExtents = new Vector3(1,1,1);
    [SerializeField] private Vector3 offset;
    private GameObject _gameObject;
    private Transform _transform;

    private List<GameObject> _interactedGameObjects = new List<GameObject>();

    [SerializeField] private LayerMask targetLayer;

    private void Awake()
    {
        _gameObject = this.gameObject;
        _transform = _gameObject.transform;
    }

    private void Update()
    {
        var position = transform.position;
        position.y += halfExtents.y;
        position += offset;
        var results = Physics.OverlapBox(position, halfExtents, Quaternion.identity, targetLayer);

        List<IInteractable> interactables = new List<IInteractable>();
        List<GameObject> newGameObjects = new List<GameObject>();

        if (results.Length > 0)
        {
            for (int i = 0; i < results.Length; i++)
            {
                var col = results[i];
                if (!includeSelf && col.gameObject == _gameObject) continue;
                interactables.AddList(col.GetComponents<IInteractable>().ToList());
                newGameObjects.Add(col.gameObject);
            }

            for (int i = 0; i < newGameObjects.Count; i++)
            {
                var obj = newGameObjects[i];
                IInteractable interactable = obj.GetComponent<IInteractable>();
                if (interactable == null) continue;
                EnterCheck(interactable, obj);
                StayCheck(interactable, obj);
            }
        }

        ExitCheck(newGameObjects);

        _interactedGameObjects = newGameObjects;
    }

    private void ExitCheck(List<GameObject> gameObjects)
    {
        for (int i = 0; i < _interactedGameObjects.Count; i++)
        {
            var target = _interactedGameObjects[i];
            if (target == null || gameObjects.Contains(target)) continue;

            var interactables = target.GetComponents<IInteractable>();

            for (int a = 0; a < interactables.Length; a++)
            {
                var interactable = interactables[a];
                interactable.OnRangeExit(_gameObject);
                onObjectExit.Invoke(target);
            }
        }
    }

    private void StayCheck(IInteractable interactable, GameObject obj)
    {
        if (obj == null)
            return;
        interactable.OnRangeStay(_gameObject);
        onObjectStayed.Invoke(obj);
    }

    private void EnterCheck(IInteractable interactable, GameObject aGameObject)
    {
        if (_interactedGameObjects.Contains(aGameObject)) return;
        interactable.OnRangeEnter(_gameObject);
        onObjectEntered.Invoke(aGameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, halfExtents);
    }
}