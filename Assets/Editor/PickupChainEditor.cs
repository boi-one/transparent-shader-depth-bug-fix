using System.Collections.Generic;
using Pathfinding.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CustomEditor( typeof( Transform ) )]
public class PickupChainEditor : Editor
{
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private Texture buttonImage;
    [SerializeField] private Vector3 offset = new Vector3(5f, 5f, 0);
     
    private void OnSceneGUI()
    { 
        var pickupSystem = PickupSystem.Instance;
        if (pickupSystem == null)
        {
            Debug.LogWarning("PickupSystem is null");
            return;
        }
        
        for (int i = pickupSystem.PickupChains.Count - 1; i >= 0; i--)
        {
            var currentChain = pickupSystem.PickupChains[i];
        
            if (currentChain.pickups.IsEmpty())
            {
                pickupSystem.PickupChains.Remove(currentChain);
                continue;
            }
            for (int j = 0; j <  currentChain.pickups.Count; j++)
            {
                var currentPickup = currentChain.pickups[j];
            
                if (currentPickup == null)
                {
                    currentChain.pickups.Remove(currentPickup);
                    continue;
                }
                if (HandleAddButton(currentPickup))
                {
                    CreateNewPickup(currentChain.pickups,j);
                }

                if(!currentChain.IsOrdered)continue;

                if(j == 0)continue;
                DrawLine( currentChain,j);
            }
        }
        
    }

    private void CreateNewPickup(List<Pickup> pickups, int currentIndex)
    {
        var newPrefab = PrefabUtility.InstantiatePrefab(pickupPrefab) as GameObject;

        newPrefab.transform.position = pickups[currentIndex].transform.position + offset;
        newPrefab.transform.parent = pickups[currentIndex].transform.parent;
        pickups.Insert(currentIndex + 1, newPrefab.GetComponent<Pickup>());
        Selection.objects = new Object[] { newPrefab };
    }

    private void DrawLine(PickupChain chain, int currentIndex)
    {
        Handles.DrawLine(chain.pickups[currentIndex - 1].transform.position, chain.pickups[currentIndex].transform.position, 7f);
    }

    private bool HandleAddButton(Pickup pickup)
    {
        if (Camera.current == null) return false;

        var isPressed = false;
        var screenPosition = HandleUtility.WorldToGUIPoint(pickup.transform.position);
        Handles.BeginGUI();

       if (GUI.Button(new Rect(screenPosition.x, screenPosition.y, 20, 20), buttonImage))
       {
           isPressed = true;
       }
        Handles.EndGUI();
        return isPressed;

    }
}
