using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version2 
{
[CreateAssetMenu(fileName = "Shape_", menuName = "ScriptableObject/V2/TileShape", order = 3)]
public class TileShape : ScriptableObject
{
    public string shapeName;
    public Side[] sides;
}

[Serializable]
public class Side
{
    public Face[] faces;
    public Vector3Int neighbourDirection;
    public SideType sideType;
}

[Serializable]
public class Face
{
    public string direction;
    public int submesh;
    public Vector3 normal;
    public Vertices[] vertices;
    public int[] triangles;
}

[Serializable]
public class Vertices
{
    public Vector3 position;
    public Vector2 uv;
}

[Serializable]
public enum SideType
{
    Empty,
    Cube,
    StairSide,
    StairFront,
    CliffSide,
    CliffSideSlope,
    CliffFront
}
}