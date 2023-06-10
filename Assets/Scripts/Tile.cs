using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile_", menuName = "ScriptableObject/TerrainTile", order = 0)]
public class Tile : ScriptableObject
{
    public string tileID;
    public TileType tileType;
    public TileState[] states;

    public virtual bool Equals(Tile t) 
    {
        return tileID == t.tileID;
    }
}

[Serializable]
public struct TileState
{
    public GameObject model;
    public Vector3 offset;
    public Quaternion rotation;
}

[Serializable]
public enum TileType
{
    Simple,
    Line,
    Smart
}