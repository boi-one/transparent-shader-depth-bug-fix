using UnityEngine;

public class PickupActivator : MonoBehaviour
{
    [SerializeField] private string triggerName = "StageOfOrb";
    [SerializeField] private string stageAnim = "State";

    public void IfFirstOrb(Pickup targetPickup)
    {
        SetTrigger(targetPickup, 1);
        //SetState(targetPickup, 1);
  
    }
    
    /*
     public void Whendisepear(Pickup targetPickup)
    {
        SetState(targetPickup, "Splash");
    }
    */
    
    public void WhenPickupedUp(PickupEvent pickupEvent)
    {
        SetTrigger(pickupEvent.targetPickup, 2);
        SetState(pickupEvent.targetPickup, "Flare");

    }
    
    public void WhenArrive(Pickup targetPickup)
    {
        SetState(targetPickup, "Splash");
    }
    
    public void FinalOrb(Pickup targetPickup)
    {
        SetTrigger(targetPickup, 3);
        ResteState(targetPickup,"Flare");
        SetState(targetPickup,"Final" );
    }
    
    private void SetTrigger(Pickup targetPickup, int isActive)
    {
        var targetAnimator = targetPickup.GetComponentInChildren<OrbAnimator>();
        targetAnimator.targetAnim.SetInteger(triggerName, isActive);
        //test
    }
    
    private void SetState(Pickup targetPickup, string stage)
    {
        var targetAnimator = targetPickup.GetComponentInChildren<OrbAnimator>();
        targetAnimator.stageAnim.SetTrigger(stage);
        //test
    }
    
    private void ResteState(Pickup targetPickup, string ResetStage)
    {
        var targetAnimator = targetPickup.GetComponentInChildren<OrbAnimator>();
        targetAnimator.stageAnim.ResetTrigger(ResetStage);
        //test
    }
}
