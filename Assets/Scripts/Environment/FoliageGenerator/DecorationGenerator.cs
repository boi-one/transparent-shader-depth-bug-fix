using System.Collections.Generic;
using Extensions;
using UnityEngine;

[ExecuteInEditMode]
public class DecorationGenerator : MonoBehaviour
{
    //Size layers
    public Vector2 OffsetSmall { get; set; }
    public Vector2 OffsetMedium { get; set; }
    public Vector2 OffsetLarge { get; set; }

    //Scripts
    private Ferr2DT_PathTerrain _ferr2DtPathTerrain;
    private static DecorationRandomizer _decorationRandomizer;
    private DecorationLibrary _decorationLibrary;
    
 
    //Lists
    private List<Vector2> _points = new List<Vector2>();

    [Header("GameObjects")]
    [Tooltip("The gameObject where all decorations will be children to")]
    [SerializeField] private GameObject decorationParent;

    [Header("Randomize Numbers")] 
    [SerializeField] private int minAmountPerSegment = 1;
    [SerializeField] private int maxAmountPerSegment = 2;
    [Tooltip("divides the platform into segments ")]
    [Range(0.0f,10.0f)] [SerializeField] private float segmentSpacing = 1f;

    [Header("Layer Settings")] 
    [SerializeField] private string backgroundName = "Background";
    [SerializeField] private string foregroundName = "Foreground";
    
    [Space]
    [Tooltip("All decoration above this height will be set to 'background'")]
    [SerializeField] private float layerThreshold = 0.5f;

#if UNITY_EDITOR
    private void Update()
    {
        //Gets the scripts from the scene
        if (Application.isPlaying) return;

        if (_decorationRandomizer == null)
        {
            _decorationRandomizer = gameObject.GetOrAddComponent<DecorationRandomizer>();
        }
        
        _decorationLibrary = FindObjectOfType<DecorationLibrary>();
        _ferr2DtPathTerrain = gameObject.GetComponent<Ferr2DT_PathTerrain>();

        _decorationRandomizer.decorationLibrary = _decorationLibrary;
        _points = _ferr2DtPathTerrain.PathData.GetPoints(0);//Gets the points of the platform
    }
#endif

    public void ResetDecoration()
    {
        RemoveDecoration();
        
        var l = _points.Count;
        for (var i = 0; i < l; i++)
        {
            var currentPoint = _points[i];
            var nextPoint = i == _points.Count - 1 ? _points[0] : _points[i+1];
            var distanceBetweenPoints = Vector2.Distance(currentPoint, nextPoint);
            var segmentLength = distanceBetweenPoints / segmentSpacing;
            
            var direction = GetDirection(currentPoint, nextPoint);

            var maxAmount = Mathf.RoundToInt(segmentLength * maxAmountPerSegment); 
            var minAmount = Mathf.RoundToInt(segmentLength * minAmountPerSegment);
            
            var k = _decorationLibrary.database.Length;
            for (var j = 0; j < k; j++)
            {
                if (_decorationLibrary.database[j].direction != direction) continue;
                _decorationRandomizer.SetDecoration(direction);
                _decorationRandomizer.RandomizeDecorationPlacement(currentPoint, nextPoint, maxAmount, minAmount, segmentSpacing , decorationParent, OffsetLarge, OffsetMedium, OffsetSmall, layerThreshold, backgroundName, foregroundName );
            }
        }
    }

    public void RemoveDecoration()
    {
        var children = new List<GameObject>();
        foreach (Transform child in decorationParent.transform) children.Add(child.gameObject);
        children.ForEach(DestroyImmediate);
    }

    private Ferr2DT_TerrainDirection GetDirection( Vector2 p1, Vector2 p2)
    {
        return Ferr2D_Path.GetDirection(p1,p2);
    }

}