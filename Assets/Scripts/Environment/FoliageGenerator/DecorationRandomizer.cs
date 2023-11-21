using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorationRandomizer : MonoBehaviour
{
    public DecorationLibrary decorationLibrary;
    private List<DecorationSettings> _decorations;
    private DecorationSettings _currentDecorationSetting;

    private float HeightOffset = 0;


    public DecorationSettings GetDecorationSettings()
    {
        var randomIndex = Random.Range(0, _decorations.Count);

        _currentDecorationSetting = _decorations[randomIndex];
        
        return _currentDecorationSetting;
    }
    
    public void RandomizeDecorationPlacement(Vector3 p1, Vector3 p2, int max, int min, float density ,GameObject decorationParent, Vector2 offsetLarge, Vector2 offsetMedium, Vector2 offsetSmall, float number, string background, string foreground)
    {
        if (_decorations == null)return;

        var newAmount = Random.Range(min, max);

        for (int i = 0; i < newAmount; i++)
        {
            var randomPosition = p1 + Random.Range (0.01f, 0.99f) * (p2 - p1);
            randomPosition += decorationParent.transform.position;

            float angleSlope = GetAngle(p1, p2);

            var decorationSettings = GetDecorationSettings();
                
            var decoration = decorationSettings.prefab;
            var decorationOffset =  GetRandomisedOffset(offsetLarge, offsetMedium,offsetSmall);
            var direction = new Vector3(0, 0, -angleSlope) + RandomiseRotation(decorationSettings);
            randomPosition = RandomisePosition(_currentDecorationSetting.minY, _currentDecorationSetting.maxY, decorationOffset, randomPosition);
            decoration.transform.localScale = RandomiseSize(decorationSettings);
            var newDecoration = Instantiate(decoration, randomPosition,Quaternion.Euler(direction), decorationParent.transform);
                
            ChangeLayer(newDecoration, HeightOffset, number, background,foreground );
        }
    }

    private void ChangeLayer(GameObject decoration, float heightOffset, float layerThreshold, string background, string foreground)
    {
        var layerName = heightOffset > layerThreshold ? background : foreground;

        if (LayerMask.NameToLayer(layerName) < 0) return;
        decoration.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in decoration.transform) child.gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public float GetRandomisedOffset(Vector2 offsetLarge, Vector2 offsetMedium, Vector2 offsetSmall)
    {
        var sizeOffset = new Dictionary<DecorationSize, Vector2>();
        sizeOffset.Add(DecorationSize.Large, offsetLarge);
        sizeOffset.Add(DecorationSize.Medium, offsetMedium);
        sizeOffset.Add(DecorationSize.Small, offsetSmall);

        var offset = sizeOffset[_currentDecorationSetting.decorationSize];
        return Random.Range(offset.x, offset.y);
    }
    
    public Vector3 RandomisePosition(float minY, float maxY,float offset, Vector3 randomPosition)
    {
        var decorationOffset = offset;
        var newHeight = Random.Range(minY, maxY);
        HeightOffset = newHeight;
        
        randomPosition += new Vector3(0, newHeight, decorationOffset);
        
        return randomPosition;
    }
    
    public Vector3 RandomiseSize(DecorationSettings decorationSettings)
    {
        var newSize = new Vector3(1,1,1);
        int l = decorationLibrary.database.Length;
        for (int i = 0; i < l; i++)
        {
            foreach (var decorationPrefab in decorationLibrary.database[i].decorationSettings)
            {
                var newSizeX = Random.Range(decorationSettings.minSize.x, decorationSettings.maxSize.x);
                var newSizeY = Random.Range(decorationSettings.minSize.y, decorationSettings.maxSize.y);

                newSize = new Vector3(newSizeX, newSizeY, 1f);
            }
        }

        return newSize;
    }

    public Vector3 RandomiseRotation(DecorationSettings decorationSettings)
    {
        var newRotation = Vector3.zero;
        
        int l = decorationLibrary.database.Length;
        for (int i = 0; i < l; i++)
        {
            foreach (var decorationPrefab in decorationLibrary.database[i].decorationSettings)
            {
                var rotationAdd = Random.Range(decorationSettings.randomRotation.x, decorationSettings.randomRotation.y);

                if (!decorationSettings.x) newRotation.x = rotationAdd;
                if(!decorationSettings.y) newRotation.y = rotationAdd;
                if(!decorationSettings.z) newRotation.z = rotationAdd;
            }
        }
        return newRotation;
    }

    public float GetAngle(Vector2 p1, Vector2 p2)
    {
        var normal = Vector2.Perpendicular(p2 - p1).normalized;
        var angleSlope = Vector3.Angle(normal, Vector3.up);
        if (p2.y > p1.y) angleSlope = -angleSlope;
        return angleSlope -180;
    }
    
    public void SetDecoration(Ferr2DT_TerrainDirection dir)
    {
        _decorations = new List<DecorationSettings>();
        _decorations = decorationLibrary.GetDecorationPrefabs(dir);
    }
}
