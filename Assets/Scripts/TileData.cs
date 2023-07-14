using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    [CreateAssetMenu(fileName = "Tile_", menuName = "TileData", order = 0)]
    public class TileData : ScriptableObject
    {
        public string tileName;
        public Mesh[] meshes;
        public Material[] materials;
    }
}
