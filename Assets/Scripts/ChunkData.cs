using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    [CreateAssetMenu(fileName = "Chunk_", menuName = "ChunkData", order = 1)]
    public class ChunkData : ScriptableObject
    {
        public Vector3Int chunkCoord;
        public Sprite[] maps;
    }
}
