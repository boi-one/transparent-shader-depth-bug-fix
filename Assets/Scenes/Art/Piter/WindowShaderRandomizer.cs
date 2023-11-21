using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowShaderRandomizer : MonoBehaviour
{
    private Material _windowShader;
    [SerializeField]private Vector2 detailSizeMinMax;
    [SerializeField] private Vector2 opacityMinMax;
    [SerializeField] private Vector2 blendMinMax;
    [SerializeField] private Vector2 offsetMinMax;
    [SerializeField] private Vector2 featherMinMax;


    private void Awake()
    {
        _windowShader = gameObject.GetComponent<Renderer>().material;

    }
    private void Start()
    {
        InvokeRepeating("RandomizeWindowShader", 0, 1);
    }

    private void RandomizeWindowShader()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("WindowDetailTextures");
        Texture2D chosenTexture = textures[Random.Range(0, textures.Length)];

        _windowShader.SetTexture("_MainTex", chosenTexture);

        float randomSize = Random.Range(detailSizeMinMax.x, detailSizeMinMax.y);
        _windowShader.SetFloat("_DetailSize", randomSize);

        float randomOpacity = Random.Range(opacityMinMax.x, opacityMinMax.y);
        _windowShader.SetFloat("_Opacity", randomOpacity);

        float randomBlend = Random.Range(blendMinMax.x, blendMinMax.y);
        _windowShader.SetFloat("_Blend", randomBlend);

        float randomFeather = Random.Range(featherMinMax.x, featherMinMax.y);
        _windowShader.SetFloat("_Feather", randomFeather);

        Vector2 randomOffset = new Vector2(Random.Range(offsetMinMax.x, offsetMinMax.y), Random.Range(offsetMinMax.x, offsetMinMax.y));
        _windowShader.SetFloat("_Detail_X_Offset", randomOffset.x);
        _windowShader.SetFloat("_Detail_Y_Offset", randomOffset.y);
    }
}
