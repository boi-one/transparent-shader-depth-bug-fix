using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshakeMediator : MonoBehaviour
{
    public void Shake(string type)
    {
        ScreenshakeSystem.Instance.Shake(type);
    }
}
