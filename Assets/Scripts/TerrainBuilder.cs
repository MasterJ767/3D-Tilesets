using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    public class TerrainBuilder : MonoBehaviour
    {
        public TileData[] tileData;
        public ChunkData[] chunkData;

        private Camera mainCamera;
        private int chunkWidth = 64;
        private int chunkHeight = 16;
        private Dictionary<Vector3Int, Chunk> chunkMap;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Start()
        {
            chunkMap = new Dictionary<Vector3Int, Chunk>();
            CreateChunks();
            PopulateChunks();
            RenderChunks();
        }

        private void Update()
        {

        }

        private void CreateChunks()
        {
            foreach (ChunkData chunk in chunkData)
            {
                CreateChunk(chunk);
            }
        }

        private void CreateChunk(ChunkData chunkData)
        {
            Vector3Int chunkPos = new Vector3Int(chunkData.chunkCoord.x * chunkWidth, chunkData.chunkCoord.y * chunkHeight, chunkData.chunkCoord.z * chunkWidth);
            GameObject newChunk = new GameObject("Chunk (" + chunkPos.x + ", " + chunkPos.y + ", " + chunkPos.z + ")");
            newChunk.transform.position = chunkPos;
            newChunk.transform.parent = transform;

            Chunk chunk = newChunk.AddComponent<Chunk>();
            chunk.Initialise(chunkPos, this, chunkData.maps);

            chunkMap.Add(chunkPos, chunk);
        }

        private void PopulateChunks()
        {
            foreach (KeyValuePair<Vector3Int, Chunk> chunk in chunkMap)
            {
                for (int i = 0; i < tileData.Length; ++i)
                {
                    chunk.Value.CreateTileMesh(i, tileData[i], chunkWidth, chunkHeight);
                }
            }
        }

        private void RenderChunks()
        {
            foreach (KeyValuePair<Vector3Int, Chunk> chunk in chunkMap)
            {
                chunk.Value.RenderTiles();
            }
        }
    }
}