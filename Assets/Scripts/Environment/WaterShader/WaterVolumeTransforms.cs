#region Using statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

#endregion

public class WaterVolumeTransforms : WaterVolumeBase
{
    #region MonoBehaviour events
    
    private void OnDrawGizmos()
    {
        if (!showDebug)
        {
            return;
        }

        // iterate the chldren
        for (var i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != "marker") continue;
            // grab the local position/scale
            var pos = transform.GetChild(i).localPosition;
            var sca = transform.GetChild(i).localScale / tileSize;

            // fix to the grid
            var x = Mathf.RoundToInt(pos.x / tileSize);
            var y = Mathf.RoundToInt(pos.y / tileSize);
            var z = Mathf.RoundToInt(pos.z / tileSize);

            var drawPos = new Vector3(x, y, z) * tileSize;
            var drawSca = new Vector3(Mathf.RoundToInt(sca.x), Mathf.RoundToInt(sca.y), Mathf.RoundToInt(sca.z)) * tileSize;
            drawPos += drawSca / 2f;
            drawPos += transform.position;
            drawPos -= new Vector3(tileSize, tileSize, tileSize);

            // render as wired volumes
            Gizmos.DrawWireCube(drawPos, drawSca);
        }
    }

    private void OnTransformChildrenChanged()
    {
        Rebuild();
    }

    #endregion

    #region Public methods

    protected override void GenerateTiles(ref bool[,,] _tiles)
    {
        // iterate the chldren
        for (var i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != "marker") continue;
            // grab the local position/scale
            var pos = transform.GetChild(i).localPosition;
            var sca = transform.GetChild(i).localScale / tileSize;

            // fix to the grid
            var x = Mathf.RoundToInt(pos.x / tileSize);
            var y = Mathf.RoundToInt(pos.y / tileSize);
            var z = Mathf.RoundToInt(pos.z / tileSize);

            // iterate the size of the transform
            for (var ix = x; ix < x + Mathf.RoundToInt(sca.x); ix++)
            {
                for (var iy = y; iy < y + Mathf.RoundToInt(sca.y); iy++)
                {
                    for (var iz = z; iz < z + Mathf.RoundToInt(sca.z); iz++)
                    {
                        // validate
                        if (ix < 0 || ix >= MAX_TILES_X || iy < 0 | iy >= MAX_TILES_Y || iz < 0 || iz >= MAX_TILES_Z)
                        {
                            continue;
                        }

                        // add the tile
                        _tiles[ix, iy, iz] = true;
                    }
                }
            }
        }
    }

    #endregion
}
