using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shape_", menuName = "ScriptableObject/TileShape", order = 0)]
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
}

[Serializable]
public class Face
{
    public string direction;
    public int submesh;
    public Vector3Int normal;
    public Vertices[] vertices;
    public int[] triangles;
}

[Serializable]
public class Vertices
{
    public Vector3 position;
    public Vector2 uv;
}