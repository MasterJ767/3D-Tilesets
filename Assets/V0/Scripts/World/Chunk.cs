using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Tileset tileset;
}

[Serializable]
public struct Tileset {
    public Transform flat;
    public Transform edge;
    public Transform edge_under;
    public Transform corner;
    public Transform corner_under;
}