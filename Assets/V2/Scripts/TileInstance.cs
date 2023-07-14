using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version2
{
[CreateAssetMenu(fileName = "Tile_", menuName = "ScriptableObject/V2/TileInstance", order = 4)]
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
    SmartLine,
    SmartRect,
    DirectionalLine,
}
}