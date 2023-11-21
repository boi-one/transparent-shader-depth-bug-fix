#region Using statements

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class WaterVolumeBase : MonoBehaviour
{
    #region Constants

    public const int MAX_TILES_X = 100;
    public const int MAX_TILES_Y = 50;
    public const int MAX_TILES_Z = 100;

    #endregion

    #region Flag lists

    [System.Flags]
    public enum TileFace : int
    {
        NegX = 1,
        PosX = 2,
        NegZ = 4,
        PosZ = 8
    }

    #endregion

    #region Private fields

    protected bool _isDirty = true;

    private Mesh _mesh = null;
    private MeshFilter _meshFilter = null;

    private bool[,,] _tiles = null;

    private Transform _childMarker;

    #endregion

    #region Public fields

    public TileFace includeFaces = TileFace.NegX | TileFace.NegZ | TileFace.PosX | TileFace.PosZ;

    public TileFace includeFoam = TileFace.NegX | TileFace.NegZ | TileFace.PosX | TileFace.PosZ;

    [Range(0.1f, 100f)] public float tileSize = 1f;

    public bool showDebug = true;
    public bool updateWaterMesh = false;

    #endregion

    #region Private methods

    private void OnEnable()
    {
        _childMarker = transform.GetChild(0);
    }

    private void EnsureReferences()
    {
        // ensure a mesh filter
        if (_meshFilter == null)
        {
            _mesh = null;
            _meshFilter = gameObject.GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }

        // ensure a mesh
        if (_mesh == null)
        {
            _mesh = _meshFilter.sharedMesh;
            if (_mesh == null || _mesh.name != "WaterVolume-" + gameObject.GetInstanceID())
            {
                _mesh = new UnityEngine.Mesh();
                _mesh.name = "WaterVolume-" + gameObject.GetInstanceID();
            }
        }

        // apply the mesh to the filter
        _meshFilter.sharedMesh = _mesh;
    }

    #endregion

    #region Public methods

    public float? GetHeight(Vector3 _position)
    {
        // convert the position to a tile
        var x = Mathf.FloorToInt((_position.x - transform.position.x + 0.5f) / tileSize);
        var z = Mathf.FloorToInt((_position.z - transform.position.z + 0.5f) / tileSize);

        // check if out of bounds
        if (x < 0 || x >= MAX_TILES_X || z < 0 || z >= MAX_TILES_Z)
        {
            return null;
        }

        // find the highest active water block in the column
        // TODO : could be reworked to cater for gaps
        for (var y = MAX_TILES_Y - 1; y >= 0; y--)
        {
            if (_tiles[x, y, z])
            {
                return transform.position.y + y * tileSize;
            }
        }

        // no water in the column
        return null;
    }

    public void Rebuild()
    {
        // ensure references to components before trying to use them
        EnsureReferences();

        // delete any existing mesh
        _mesh.Clear();

        // allow any child class to generate the tiles to build from
        _tiles = new bool[MAX_TILES_X, MAX_TILES_Y, MAX_TILES_Z];
        GenerateTiles(ref _tiles);

        // prepare buffers for the mesh data
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var colors = new List<Color>();
        var indices = new List<int>();
      
        
        // iterate the tiles
        for (var x = 0; x < MAX_TILES_X; x++)
        {
            for (var y = 0; y < MAX_TILES_Y; y++)
            {
                for (var z = 0; z < MAX_TILES_Z; z++)
                {
                    // check there is water here
                    if (!_tiles[x, y, z])
                    {
                        continue;
                    }

                    // calculate tile position
                    var x0 = x * tileSize - 0.5f;
                    var x1 = x0 + tileSize;
                    var y0 = y * tileSize - 0.5f;
                    var y1 = y0 + tileSize;
                    var z0 = z * tileSize - 0.5f;
                    var z1 = z0 + tileSize;
                    var ux0 = x0 + transform.position.x;
                    var ux1 = x1 + transform.position.x;
                    var uy0 = y0 + transform.position.y;
                    var uy1 = y1 + transform.position.y;
                    var uz0 = z0 + transform.position.z;
                    var uz1 = z1 + transform.position.z;

                    // check for edges
                    var negX = x == 0 || !_tiles[x - 1, y, z];
                    var posX = x == MAX_TILES_X - 1 || !_tiles[x + 1, y, z];
                    var negY = y == 0 || !_tiles[x, y - 1, z];
                    var posY = y == MAX_TILES_Y - 1 || !_tiles[x, y + 1, z];
                    var negZ = z == 0 || !_tiles[x, y, z - 1];
                    var posZ = z == MAX_TILES_Z - 1 || !_tiles[x, y, z + 1];
                    var negXnegZ = !negX && !negZ && x > 0 && z > 0 && !_tiles[x - 1, y, z - 1];
                    var negXposZ = !negX && !posZ && x > 0 && z < MAX_TILES_Z && !_tiles[x - 1, y, z + 1];
                    var posXposZ = !posX && !posZ && x < MAX_TILES_X && z < MAX_TILES_Z && !_tiles[x + 1, y, z + 1];
                    var posXnegZ = !posX && !negZ && x < MAX_TILES_X && z > 0 && !_tiles[x + 1, y, z - 1];
                    var faceNegX = negX && (includeFaces & TileFace.NegX) == TileFace.NegX;
                    var facePosX = posX && (includeFaces & TileFace.PosX) == TileFace.PosX;
                    var faceNegZ = negZ && (includeFaces & TileFace.NegZ) == TileFace.NegZ;
                    var facePosZ = posZ && (includeFaces & TileFace.PosZ) == TileFace.PosZ;
                    var foamNegX = negX && (includeFoam & TileFace.NegX) == TileFace.NegX;
                    var foamPosX = posX && (includeFoam & TileFace.PosX) == TileFace.PosX;
                    var foamNegZ = negZ && (includeFoam & TileFace.NegZ) == TileFace.NegZ;
                    var foamPosZ = posZ && (includeFoam & TileFace.PosZ) == TileFace.PosZ;
                    var foamNegXnegZ = negXnegZ && ((includeFoam & TileFace.NegX) == TileFace.NegX ||
                                                    (includeFoam & TileFace.NegZ) == TileFace.NegZ);
                    var foamNegXposZ = negXposZ && ((includeFoam & TileFace.PosX) == TileFace.PosX ||
                                                    (includeFoam & TileFace.PosZ) == TileFace.PosZ);
                    var foamPosXposZ = posXposZ && ((includeFoam & TileFace.NegZ) == TileFace.NegZ ||
                                                    (includeFoam & TileFace.PosZ) == TileFace.PosZ);
                    var foamPosXnegZ = posXnegZ && ((includeFoam & TileFace.PosZ) == TileFace.PosZ ||
                                                    (includeFoam & TileFace.NegZ) == TileFace.NegZ);

                    // create the top face
                    if (y == MAX_TILES_Y - 1 || !_tiles[x, y + 1, z])
                    {
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x1, y1, z0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        normals.Add(new Vector3(0, 1, 0));
                        uvs.Add(new Vector2(ux0, uz0));
                        uvs.Add(new Vector2(ux0, uz1));
                        uvs.Add(new Vector2(ux1, uz1));
                        uvs.Add(new Vector2(ux1, uz0));

                        var color1 = foamNegX || foamNegZ || foamNegXnegZ ? Color.red : Color.black;
                        var color2 = foamNegX || foamPosZ || foamNegXposZ ? Color.red : Color.black;
                        var color3 = foamPosX || foamPosZ || foamPosXposZ ? Color.red : Color.black;
                        var color4 = foamPosX || foamNegZ || foamPosXnegZ ? Color.red : Color.black;

                        if (negZ)
                        {
                            color1.b = 1;
                            color4.b = 1;
                        }

                        colors.Add(color1);
                        colors.Add(color2);
                        colors.Add(color3);
                        colors.Add(color4);

                        var v = vertices.Count - 4;
                        if (foamNegX && foamPosZ || foamPosX && foamNegZ)
                        {
                            indices.Add(v + 1);
                            indices.Add(v + 2);
                            indices.Add(v + 3);
                            indices.Add(v + 3);
                            indices.Add(v);
                            indices.Add(v + 1);
                        }
                        else
                        {
                            indices.Add(v);
                            indices.Add(v + 1);
                            indices.Add(v + 2);
                            indices.Add(v + 2);
                            indices.Add(v + 3);
                            indices.Add(v);
                        }
                    }

                    // create the side faces
                    if (faceNegX)
                    {
                        vertices.Add(new Vector3(x0, y0, z1));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x0, y0, z0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        normals.Add(new Vector3(-1, 0, 0));
                        uvs.Add(new Vector2(uz1, uy0));
                        uvs.Add(new Vector2(uz1, uy1));
                        uvs.Add(new Vector2(uz0, uy1));
                        uvs.Add(new Vector2(uz0, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }

                    if (facePosX)
                    {
                        vertices.Add(new Vector3(x1, y0, z0));
                        vertices.Add(new Vector3(x1, y1, z0));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x1, y0, z1));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        normals.Add(new Vector3(1, 0, 0));
                        uvs.Add(new Vector2(uz0, uy0));
                        uvs.Add(new Vector2(uz0, uy1));
                        uvs.Add(new Vector2(uz1, uy1));
                        uvs.Add(new Vector2(uz1, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }

                    if (faceNegZ)
                    {
                        vertices.Add(new Vector3(x0, y0, z0));
                        vertices.Add(new Vector3(x0, y1, z0));
                        vertices.Add(new Vector3(x1, y1, z0));
                        vertices.Add(new Vector3(x1, y0, z0));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        normals.Add(new Vector3(0, 0, -1));
                        uvs.Add(new Vector2(ux0, uy0));
                        uvs.Add(new Vector2(ux0, uy1));
                        uvs.Add(new Vector2(ux1, uy1));
                        uvs.Add(new Vector2(ux1, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? new Color(1, 0, 1, 1) : Color.black);
                        colors.Add(posY ? new Color(1, 0, 1, 1) : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }

                    if (facePosZ)
                    {
                        vertices.Add(new Vector3(x1, y0, z1));
                        vertices.Add(new Vector3(x1, y1, z1));
                        vertices.Add(new Vector3(x0, y1, z1));
                        vertices.Add(new Vector3(x0, y0, z1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        normals.Add(new Vector3(0, 0, 1));
                        uvs.Add(new Vector2(ux1, uy0));
                        uvs.Add(new Vector2(ux1, uy1));
                        uvs.Add(new Vector2(ux0, uy1));
                        uvs.Add(new Vector2(ux0, uy0));
                        colors.Add(Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(posY ? Color.red : Color.black);
                        colors.Add(Color.black);
                        var v = vertices.Count - 4;
                        indices.Add(v);
                        indices.Add(v + 1);
                        indices.Add(v + 2);
                        indices.Add(v + 2);
                        indices.Add(v + 3);
                        indices.Add(v);
                    }
                }
            }
        }

        // apply the buffers
        _mesh.SetVertices(vertices);
        _mesh.SetNormals(normals);
        _mesh.SetUVs(0, uvs);
        _mesh.SetColors(colors);
        _mesh.SetTriangles(indices, 0);

        // update
        _mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        _mesh.RecalculateTangents();

        _meshFilter.sharedMesh = _mesh;

        // flag as clean
        _isDirty = false;
    }

    #endregion

    #region Virtual methods

    protected virtual void GenerateTiles(ref bool[,,] _tiles)
    {
    }

    public virtual void Validate()
    {
    }

    #endregion

    #region MonoBehaviour events

  
    void OnValidate()
    {
        // keep tile size in a sensible range
        tileSize = Mathf.Clamp(tileSize, 0.1f, 100f);

        GetComponent<Renderer>().sharedMaterial.SetFloat("_TileSize", tileSize);

        // allow any child class to perform validation
        Validate();

        // flag as needing rebuilding
        _isDirty = true;
    }

    private protected virtual void Update()
    {
        // rebuild if needed
        if (_isDirty || (!Application.isPlaying && updateWaterMesh))
        {
            Rebuild();
        }
    }

    #endregion
}