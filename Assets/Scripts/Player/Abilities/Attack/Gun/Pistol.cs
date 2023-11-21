using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : Weapon
{
    public override bool Use(InputAction.CallbackContext context)
    {
        Shoot(context);
        return true;
    }
    
    public override void OnInput(InputAction.CallbackContext aContext)
    {
        Use(aContext);
        GetComponentInChildren<StateMachine.StateMachine>().SetBool("Attacking", true);
    }
    
    // todo: Once 360aiming is merged with Develop, remove this and use that one in Slingshot.cs instead
    public Vector3 GetShootDir()
    {
        return shootDir;
    }
    // !todo
}