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
                    FillColumn(i, x, z, tileMesh);
                }
            }
        }

        public void FillColumn(int i, int x, int z, TileMesh tileMesh)
        {
            float r = maps[i].texture.GetPixel(x, z).r;
            float g = maps[i].texture.GetPixel(x, z).g;
            float b = maps[i].texture.GetPixel(x, z).b;
            int mode = Mathf.RoundToInt((1f - r) * 16f) - 1;
            int min = Mathf.Max(Mathf.RoundToInt((1f - g) * 16f) - 1, 0);
            int max = Mathf.Max(Mathf.RoundToInt((1f - b) * 16f) - 1, 0);

            switch (mode)
            {
                case -1: // ignore column
                    break;
                case 0: // fill column between green and blue value, leaving other values empty for air
                    for (int y = min; y <= max; ++y)
                    {
                        tileMesh.AddTile(new Vector3Int(x, y, z), 0);
                    }
                    break;
                case 1: // fill column, leaving values between green and blue empty for air
                    for (int y = 0; y < min; ++y)
                    {
                        tileMesh.AddTile(new Vector3Int(x, y, z), 0);
                    }
                    for (int y = max + 1; y < chunkHeight; ++y)
                    {
                        tileMesh.AddTile(new Vector3Int(x, y, z), 0);
                    }
                    break;
                case 2: // place at height green facing north
                    tileMesh.AddTile(new Vector3Int(x, min, z), 0);
                    break;
                case 3: // place at height green facing east
                    tileMesh.AddTile(new Vector3Int(x, min, z), 90);
                    break;
                case 4: // place at height green facing south
                    tileMesh.AddTile(new Vector3Int(x, min, z), 180);
                    break;
                case 5: // place at height green facing west
                    tileMesh.AddTile(new Vector3Int(x, min, z), 270);
                    break;
                default: 
                    break;
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

        public bool GetNeighbour(Vector3Int pos, TileCategory category)
        {
            if (pos.x >= 0 && pos.x < chunkWidth && pos.y >= 0 && pos.y < chunkHeight && pos.z >= 0 && pos.z < chunkWidth)
            {
                return SearchMeshesForNeighbour(pos, this, category);
            }
            
            Vector3Int neighbourGlobalPos = new Vector3Int(chunkPos.x + pos.x, chunkPos.y + pos.y, chunkPos.z + pos.z);
            Vector3Int neighbourChunkPos = new Vector3Int(Mathf.FloorToInt(neighbourGlobalPos.x / (float)chunkWidth) * chunkWidth, Mathf.FloorToInt(neighbourGlobalPos.y / (float)chunkHeight) * chunkHeight, Mathf.FloorToInt(neighbourGlobalPos.z / (float)chunkWidth) * chunkWidth);
            Chunk neighbourChunk = terrainBuilder.GetChunk(neighbourChunkPos);
            Vector3Int neighbourLocalPos = neighbourGlobalPos - neighbourChunkPos;
            if (neighbourChunk == null) { return true; }
            return SearchMeshesForNeighbour(neighbourLocalPos, neighbourChunk, category);
        }

        private bool SearchMeshesForNeighbour(Vector3Int pos, Chunk chunk, TileCategory category)
        {
            foreach (TileMesh mesh in chunk.meshes)
            {
                if (mesh == null || mesh.GetTileCategory() != category) { continue; }
                if (mesh.GetTile(pos).present) { return true; }
            }
            return false;
        }

        public void UpdateTile(Vector3Int pos, int i, int rotation, TileCategory category)
        {
            Chunk chunk;
            Vector3Int position;
            if (pos.x >= 0 && pos.x < chunkWidth && pos.y >= 0 && pos.y < chunkHeight && pos.z >= 0 && pos.z < chunkWidth)
            {
                chunk = this;
                position = pos;
            }
            else
            {
                Vector3Int neighbourGlobalPos = new Vector3Int(chunkPos.x + pos.x, chunkPos.y + pos.y, chunkPos.z + pos.z);
                Vector3Int neighbourChunkPos = new Vector3Int(Mathf.FloorToInt(neighbourGlobalPos.x / (float)chunkWidth) * chunkWidth, Mathf.FloorToInt(neighbourGlobalPos.y / (float)chunkHeight) * chunkHeight, Mathf.FloorToInt(neighbourGlobalPos.z / (float)chunkWidth) * chunkWidth);
                chunk = terrainBuilder.GetChunk(neighbourChunkPos);
                if (chunk == null) { return; }
                position = neighbourGlobalPos - neighbourChunkPos;
            }

            if (i >= 0)
            {
                chunk.meshes[i].UpdateTile(position, rotation);
            }
            else
            {
                foreach (TileMesh mesh in chunk.meshes)
                {
                    if (mesh == null || mesh.GetTileCategory() != category) { continue; }
                    if (mesh.GetTile(position).present) { mesh.UpdateTile(position, rotation); }
                }
            }
        }
    }
}
