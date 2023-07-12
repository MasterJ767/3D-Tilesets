using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile_", menuName = "ScriptableObject/TileInstance", order = 1)]
public class TileInstance : ScriptableObject
{
    public string tileName;
    public SmartShape tileType;
    public Material[] materials;
    public TileShape[] shapes;
    public bool rotateXZface;
}

[Serializable]
public enum SmartShape
{
    Basic,
    Line,
    Rect,
    Smart
}