using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    public class Chunk : MonoBehaviour
    {
        private Vector3Int chunkPos;
        private TerrainBuilder terrainBuilder;
        private int chunkWidth;
        private int chunkHeight;
        private Sprite[] maps;
        private TileMesh[] meshes;

        public void Initialise(Vector3Int chunkPos, TerrainBuilder terrainBuilder, int chunkWidth, int chunkHeight, Sprite[] maps)
        {
            this.chunkPos = chunkPos;
            this.terrainBuilder = terrainBuilder;
            this.chunkWidth = chunkWidth;
            this.chunkHeight = chunkHeight;
            this.maps = maps;
            meshes = new TileMesh[maps.Length];
        }

        public void CreateTileMesh(int i, TileData tile)
        {
            GameObject newTileMesh = new GameObject(tile.tileName);
            newTileMesh.transform.position = chunkPos;
            newTileMesh.transform.parent = transform;

            TileMesh tileMesh = newTileMesh.AddComponent<TileMesh>();
            newTileMesh.AddComponent<MeshFilter>();
            newTileMesh.AddComponent<MeshRenderer>();
            tileMesh.Initialise(tile, this, i, chunkWidth, chunkHeight);

            meshes[i] = tileMesh;

            for (int x = 0; x < chunkWidth; ++x)
            {
                for (int z = 0; z < chunkWidth; ++z)
                {
                    float v = maps[i].texture.GetPixel(x, z).r;
                    if (v == 1f) { continue; }
                    int y = Mathf.RoundToInt((1f - v) * 16f) - 1;
                    while (y >= 0) {
                        tileMesh.AddTile(new Vector3Int(x, y, z));
                        y--;
                    }
                }
            }
        }

        public void RenderTiles()
        {
            foreach (TileMesh mesh in meshes)
            {
                if (mesh == null) { continue; }
                mesh.Render();
            }
        }

        public bool GetNeighbourTile(Vector3Int neighbourPos, int index, TileCategory category)
        {
            Vector3Int neighbourGlobalPos = new Vector3Int(chunkPos.x + neighbourPos.x, chunkPos.y + neighbourPos.y, chunkPos.z + neighbourPos.z);
            Vector3Int neighbourChunkPos = new Vector3Int(Mathf.FloorToInt(neighbourGlobalPos.x / (float)chunkWidth) * chunkWidth, Mathf.FloorToInt(neighbourGlobalPos.y / (float)chunkHeight) * chunkHeight, Mathf.FloorToInt(neighbourGlobalPos.z / (float)chunkWidth) * chunkWidth);
            Chunk neighbourChunk = terrainBuilder.GetChunk(neighbourChunkPos);
            Vector3Int neighbourLocalPos = neighbourGlobalPos - neighbourChunkPos;
            if (neighbourChunk == null) { return true; }
            return GetNeighbour(neighbourLocalPos, neighbourChunk, index, category);
        }

        public bool GetNeighbour(Vector3Int neighbourPos, Chunk chunk, int index, TileCategory category)
        {
            for (int i = 0; i < chunk.meshes.Length; ++i)
            {
                if (chunk.meshes[i] == null || i == index || chunk.meshes[i].GetTileCategory() != category) { continue; }
                if (chunk.meshes[i].GetTile(neighbourPos).present) { return true; };
            }
            return false;
        }

        public void UpdateNeighbourTile(Vector3Int neighbourPos, int index, TileCategory category)
        {
            Vector3Int neighbourGlobalPos = new Vector3Int(chunkPos.x + neighbourPos.x, chunkPos.y + neighbourPos.y, chunkPos.z + neighbourPos.z);
            Vector3Int neighbourChunkPos = new Vector3Int(Mathf.FloorToInt(neighbourGlobalPos.x / (float)chunkWidth) * chunkWidth, Mathf.FloorToInt(neighbourGlobalPos.y / (float)chunkHeight) * chunkHeight, Mathf.FloorToInt(neighbourGlobalPos.z / (float)chunkWidth) * chunkWidth);
            Chunk neighbourChunk = terrainBuilder.GetChunk(neighbourChunkPos);
            Vector3Int neighbourLocalPos = neighbourGlobalPos - neighbourChunkPos;
            if (neighbourChunk == null) { return; }
            UpdateNeighbour(neighbourLocalPos, neighbourChunk, index, category);
        }

        public void UpdateNeighbour(Vector3Int neighbourPos, Chunk chunk, int index, TileCategory category)
        {
            for (int i = 0; i < chunk.meshes.Length; ++i)
            {
                if (chunk.meshes[i] == null || i == index || chunk.meshes[i].GetTileCategory() != category) { continue; }
                if (chunk.meshes[i].GetTile(neighbourPos).present) { chunk.meshes[i].UpdateTileExternal(neighbourPos); };
            }
        }
    }
}
