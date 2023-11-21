using UnityEngine;

public interface IInteractable
{
    public void OnRangeEnter(GameObject other);
    public void OnRangeExit(GameObject other);
    public void OnRangeStay(GameObject other);
}
