using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackSystemMediator : MonoBehaviour
{
    public void OnCollision(ForceBody forceBody, RaycastHit hit)
    {
        KnockbackSystem.Instance.BounceKnockback(forceBody, hit);
    }

    public void OnKnockbackBounceCompleted(ForceBody forceBody)
    {
        KnockbackSystem.Instance.OnKnockbackBounceCompleted(forceBody);
    }
}
