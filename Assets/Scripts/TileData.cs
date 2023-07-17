using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    [CreateAssetMenu(fileName = "Tile_", menuName = "TileData", order = 0)]
    public class TileData : ScriptableObject
    {
        public string tileName;
        public TileCategory category;
        public MeshData[] meshData;
        public Material[] materials;
    }

    [Serializable]
    public enum TileCategory
    {
        Terrain,
        Stair,
        Fluid
    }

    [Serializable]
    public struct MeshData
    {
        public Mesh mesh;
        [Range(0, 2)] public int[] submeshIndices;
    }
}
