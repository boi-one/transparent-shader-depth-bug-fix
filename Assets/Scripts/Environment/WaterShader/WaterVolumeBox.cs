#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class WaterVolumeBox : WaterVolumeBase
{
    #region Public fields

    public Vector3 dimensions = Vector3.zero;

    #endregion

    #region Public methods

    protected override void GenerateTiles(ref bool[,,] _tiles)
    {
        // calculate volume in tiles
        var maxX = Mathf.Clamp(Mathf.RoundToInt(dimensions.x / tileSize), 1, MAX_TILES_X);
        var maxY = Mathf.Clamp(Mathf.RoundToInt(dimensions.y / tileSize), 1, MAX_TILES_Y);
        var maxZ = Mathf.Clamp(Mathf.RoundToInt(dimensions.z / tileSize), 1, MAX_TILES_Z);

        // populate the tiles with a box volume
        for (var x = 0; x < maxX; x++)
        {
            for (var y = 0; y < maxY; y++)
            {
                for (var z = 0; z < maxZ; z++)
                {
                    _tiles[x, y, z] = true;
                }
            }
        }
    }

    public override void Validate()
    {
        // keep values sensible
        dimensions.x = Mathf.Clamp(dimensions.x, 1, MAX_TILES_X);
        dimensions.y = Mathf.Clamp(dimensions.y, 1, MAX_TILES_Y);
        dimensions.z = Mathf.Clamp(dimensions.z, 1, MAX_TILES_Z);
    }

    #endregion
}
