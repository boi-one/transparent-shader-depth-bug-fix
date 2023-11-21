using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class LayerPriority : MonoBehaviour
{
    [SerializeField] private int frontlayer = 10;
    [SerializeField] private int backlayer = 11;
    [SerializeField] private float layertimer;
    [SerializeField] private GameObject body;
    [SerializeField] private Vector3 changeZposition = new Vector3(0, 0, 1f);

    private Coroutine LayerResetting;
    
    void Start()
    {
        gameObject.layer = backlayer;
    }


    public void SetFrontLayer()
    {

        gameObject.layer = frontlayer;
        body.transform.position -= changeZposition;
        if (LayerResetting != null) StopCoroutine(LayerResetting);
        LayerResetting = StartCoroutine(LayerCoroutine());
    }

    public void SetBackLayer()
    {
        gameObject.layer = backlayer;
        body.transform.position += changeZposition;
    }
    
    IEnumerator LayerCoroutine()
    {
        yield return new WaitForSeconds(layertimer);
        SetBackLayer();
    }
}
