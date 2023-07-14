using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    public class Chunk : MonoBehaviour
    {
        private Vector3Int chunkPos;
        private TerrainBuilder terrainBuilder;
        private Sprite[] maps;
        private TileMesh[] meshes;

        public void Initialise(Vector3Int chunkPos, TerrainBuilder terrainBuilder, Sprite[] maps)
        {
            this.chunkPos = chunkPos;
            this.terrainBuilder = terrainBuilder;
            this.maps = maps;
            meshes = new TileMesh[maps.Length];
        }

        public void CreateTileMesh(int i, TileData tile, int chunkWidth, int chunkHeight)
        {
            GameObject newTileMesh = new GameObject(tile.tileName);
            newTileMesh.transform.position = chunkPos;
            newTileMesh.transform.parent = transform;

            TileMesh tileMesh = newTileMesh.AddComponent<TileMesh>();
            newTileMesh.AddComponent<MeshFilter>();
            newTileMesh.AddComponent<MeshRenderer>();
            tileMesh.Initialise(tile, this, chunkWidth, chunkHeight);

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
    }
}
