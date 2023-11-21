#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class WaterVolumeHelper : MonoBehaviour
{
    #region Private static fields

    private static WaterVolumeHelper _instance = null;

    #endregion

    #region Public fields

    public WaterVolumeBase waterVolume = null;

    #endregion

    #region Public static properties

    public static WaterVolumeHelper Instance { get { return _instance; } }

    #endregion

    #region Public methods

    public float? GetHeight(Vector3 _position)
    {
        // ensure a water volume
        if (!waterVolume)
        {
            return 0f;
        }

        // ensure a material
        var renderer = waterVolume.gameObject.GetComponent<MeshRenderer>();
        if (!renderer || !renderer.sharedMaterial)
        {
            return 0f;
        }

        // replicate the shader logic, using parameters pulled from the specific material, to return the height at the specified position
        var waterHeight = waterVolume.GetHeight(_position);
        if (!waterHeight.HasValue)
        {
            return null;
        }
        var _WaveFrequency = renderer.sharedMaterial.GetFloat("_WaveFrequency");
        var _WaveScale = renderer.sharedMaterial.GetFloat("_WaveScale");
        var _WaveSpeed = renderer.sharedMaterial.GetFloat("_WaveSpeed");
        var time = Time.time * _WaveSpeed;
        var shaderOffset = (Mathf.Sin(_position.x * _WaveFrequency + time) + Mathf.Cos(_position.z * _WaveFrequency + time)) * _WaveScale;
        return waterHeight.Value + shaderOffset;
    }

    #endregion

    #region MonoBehaviour events

    private void Awake()
    {
        _instance = this;
    }

    #endregion
}