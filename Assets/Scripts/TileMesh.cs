using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    public class TileMesh : MonoBehaviour
    {
        private TileData tile;
        private Chunk chunk;
        private int index;
        private int width;
        private int height;
        private Tile[,,] tileMap;

        public void Initialise(TileData tile, Chunk chunk, int index, int width, int height)
        {
            this.tile = tile;
            this.chunk = chunk;
            this.index = index;
            this.width = width;
            this.height = height;
            tileMap = new Tile[width, height, width];
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < width; ++z)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        tileMap[x, y, z] = new Tile(false, 0, 0);
                    }
                }
            }
        }

        public void AddTile(Vector3Int pos)
        {
            chunk.UpdateTile(pos, index, tile.category);
            UpdateNeighbours(pos);
        }

        private void UpdateNeighbours(Vector3Int pos)
        {
            int sides = DetermineNeighbours(pos);

            if ((sides & 1) == 1) { chunk.UpdateTile(pos + new Vector3Int(0, 0, 1), -1, tile.category); }
            if ((sides & 2) == 2) { chunk.UpdateTile(pos + new Vector3Int(1, 0, 1), -1, tile.category); }
            if ((sides & 4) == 4) { chunk.UpdateTile(pos + new Vector3Int(1, 0, 0), -1, tile.category); }
            if ((sides & 8) == 8) { chunk.UpdateTile(pos + new Vector3Int(1, 0, -1), -1, tile.category); }
            if ((sides & 16) == 16) { chunk.UpdateTile(pos + new Vector3Int(0, 0, -1), -1, tile.category); }
            if ((sides & 32) == 32) { chunk.UpdateTile(pos + new Vector3Int(-1, 0, -1), -1, tile.category); }
            if ((sides & 64) == 64) { chunk.UpdateTile(pos + new Vector3Int(-1, 0, 0), -1, tile.category); }
            if ((sides & 128) == 128) { chunk.UpdateTile(pos + new Vector3Int(-1, 0, 1), -1, tile.category); }
            if ((sides & 256) == 256) { chunk.UpdateTile(pos + new Vector3Int(0, 1, 0), -1, tile.category); }
            if ((sides & 512) == 512) { chunk.UpdateTile(pos + new Vector3Int(0, -1, 0), -1, tile.category); }
        }

        private int DetermineNeighbours(Vector3Int pos)
        {
            int sides = 0;
            sides += chunk.GetNeighbour(pos + new Vector3Int(0, 0, 1), tile.category) ? 1 : 0; // front 
            sides += chunk.GetNeighbour(pos + new Vector3Int(1, 0, 1), tile.category) ? 1 << 1 : 0; // front right 
            sides += chunk.GetNeighbour(pos + new Vector3Int(1, 0, 0), tile.category) ? 1 << 2 : 0; // right
            sides += chunk.GetNeighbour(pos + new Vector3Int(1, 0, -1), tile.category) ? 1 << 3 : 0; // back right 
            sides += chunk.GetNeighbour(pos + new Vector3Int(0, 0, -1), tile.category) ? 1 << 4 : 0; // back
            sides += chunk.GetNeighbour(pos + new Vector3Int(-1, 0, -1), tile.category) ? 1 << 5 : 0; // back left
            sides += chunk.GetNeighbour(pos + new Vector3Int(-1, 0, 0), tile.category) ? 1 << 6 : 0; // left
            sides += chunk.GetNeighbour(pos + new Vector3Int(-1, 0, 1), tile.category) ? 1 << 7 : 0; // front left 
            sides += chunk.GetNeighbour(pos + new Vector3Int(0, 1, 0), tile.category) ? 1 << 8 : 0; // up
            sides += chunk.GetNeighbour(pos + new Vector3Int(0, -1, 0), tile.category) ? 1 << 9 : 0; // down
            return sides;
        }

        public void UpdateTile(Vector3Int pos)
        {
            int sides = DetermineNeighbours(pos);
            foreach (Tuple<int, int, int> i in Iterator.smartIter)
            {
                bool result = CompareChange(sides, i.Item1, pos, i.Item2, i.Item3);
                if (result) { return; }
            }
        }

        private bool CompareChange(int sides, int mask, Vector3Int pos, int index, int rotation)
        {
            if ((sides & mask) == mask)
            {
                tileMap[pos.x, pos.y, pos.z].present = true;
                tileMap[pos.x, pos.y, pos.z].index = index;
                tileMap[pos.x, pos.y, pos.z].rotation = rotation;
                return true;
            }
            return false;
        }

        public void Render()
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int>[] triangles = new List<int>[3];
            triangles[0] = new List<int>();
            triangles[1] = new List<int>();
            triangles[2] = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            int vertexCount = 0;

            List<Vector3> instanceVertices = new List<Vector3>();
            List<Vector3> instanceNormals = new List<Vector3>();
            List<int>[] instanceTriangles = new List<int>[3];
            instanceTriangles[0] = new List<int>();
            instanceTriangles[1] = new List<int>();
            instanceTriangles[2] = new List<int>();
            List<Vector2> instanceUvs = new List<Vector2>();

            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < width; ++z)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        Tile current = tileMap[x, y, z];
                        if (current.present == false || current.index < 0) { continue; }
                        Mesh instance = tile.meshData[current.index].mesh;

                        instance.GetVertices(instanceVertices);
                        for (int v = 0; v < instanceVertices.Count; ++v)
                        {
                            instanceVertices[v] = RotateYAxis(instanceVertices[v], current.rotation) + new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                        }
                        vertices.AddRange(instanceVertices);

                        instance.GetNormals(instanceNormals);
                        for (int n = 0; n < instanceVertices.Count; ++n)
                        {
                            instanceNormals[n] = RotateYAxis(instanceNormals[n], current.rotation);
                        }
                        normals.AddRange(instanceNormals);

                        for (int i = 0; i < instance.subMeshCount; ++i) {
                            instance.GetTriangles(instanceTriangles[i], i);
                            for (int t = 0; t < instanceTriangles[i].Count; ++t)
                            {
                                instanceTriangles[i][t] += vertexCount;
                            }
                            triangles[tile.meshData[current.index].submeshIndices[i]].AddRange(instanceTriangles[i]);
                        }

                        instance.GetUVs(0, instanceUvs);
                        uvs.AddRange(instanceUvs);

                        instanceVertices.Clear();
                        instanceNormals.Clear();
                        instanceTriangles[0].Clear();
                        instanceTriangles[1].Clear();
                        instanceTriangles[2].Clear();
                        instanceUvs.Clear();
                        vertexCount += instance.vertexCount;
                    }
                }
            }

            meshRenderer.materials = tile.materials;

            Mesh mesh = new Mesh();
            mesh.name = gameObject.name;
            mesh.subMeshCount = 3;
            mesh.SetVertices(vertices.ToArray());
            mesh.SetTriangles(triangles[0].ToArray(), 0);
            mesh.SetTriangles(triangles[1].ToArray(), 1);
            mesh.SetTriangles(triangles[2].ToArray(), 2);
            mesh.SetUVs(0, uvs.ToArray());
            mesh.SetNormals(normals.ToArray());
            mesh.Optimize();

            meshFilter.mesh = mesh;
        }

        private Vector3 RotateYAxis(Vector3 vector, float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector3(vector.x * Mathf.Cos(angle) + vector.z * Mathf.Sin(angle), vector.y, vector.z * Mathf.Cos(angle) - vector.x * Mathf.Sin(angle));
        }

        public TileCategory GetTileCategory()
        {
            return tile.category;
        }

        public Tile GetTile(Vector3Int pos)
        {
            return tileMap[pos.x, pos.y, pos.z];
        }
    }

    [Serializable]
    public class Tile
    {
        public bool present;
        public int index;
        public int rotation;
        
        public Tile(bool present, int index, int rotation)
        {
            this.present = present;
            this.index = index;
            this.rotation = rotation;
        }
    }

    public static class Iterator
    {
        public static readonly Tuple<int, int, int>[] smartIter = new Tuple<int, int, int>[]{
                // Has tile above
                new Tuple<int, int, int>(341, -1, 0), // F R B L U
                new Tuple<int, int, int>(325, 8, 0), // L F R U
                new Tuple<int, int, int>(277, 8, 90), // F R B U
                new Tuple<int, int, int>(340, 8, 180), // R B L U
                new Tuple<int, int, int>(337, 8, 270), // B L F U
                new Tuple<int, int, int>(261, 4, 0), // F R U
                new Tuple<int, int, int>(276, 4, 90), // R B U
                new Tuple<int, int, int>(336, 4, 180), // B L U
                new Tuple<int, int, int>(321, 4, 270), // L F U
                new Tuple<int, int, int>(257, 2, 0), // F U
                new Tuple<int, int, int>(260, 2, 90), // R U
                new Tuple<int, int, int>(272, 2, 180), // B U
                new Tuple<int, int, int>(320, 2, 270), // L U
                new Tuple<int, int, int>(256, 0, 0), // U

                // 8 horizontal adjacent
                new Tuple<int, int, int>(255, 19, 0), // All Horiz

                // 7 horizontal adjacent
                new Tuple<int, int, int>(253, 18, 0), // All Horiz except FR
                new Tuple<int, int, int>(247, 18, 90), // All Horiz except FL
                new Tuple<int, int, int>(223, 18, 180), // All Horiz except BL
                new Tuple<int, int, int>(127, 18, 270), // All Horiz except BR

                // 6 horizontal adjacent
                new Tuple<int, int, int>(245, 16, 0), // All Horiz except FR BR
                new Tuple<int, int, int>(215, 16, 90), // All Horiz except BR BL
                new Tuple<int, int, int>(91, 16, 180), // All Horiz except BL FL
                new Tuple<int, int, int>(125, 16, 270), // All Horiz except FL FR
                new Tuple<int, int, int>(221, 17, 0), // All Horiz except FR BL
                new Tuple<int, int, int>(119, 17, 90), // All Horiz except FL BR

                // 5 horizontal adjacent
                new Tuple<int, int, int>(117, 14, 0), 
                new Tuple<int, int, int>(213, 14, 90), 
                new Tuple<int, int, int>(87, 14, 180), 
                new Tuple<int, int, int>(93, 14, 270),
                new Tuple<int, int, int>(199, 15, 0), 
                new Tuple<int, int, int>(31, 15, 90), 
                new Tuple<int, int, int>(124, 15, 180), 
                new Tuple<int, int, int>(241, 15, 270),

                // 4 horizontal adjacent
                new Tuple<int, int, int>(197, 11, 0), 
                new Tuple<int, int, int>(23, 11, 90), 
                new Tuple<int, int, int>(92, 11, 180), 
                new Tuple<int, int, int>(113, 11, 270), 
                new Tuple<int, int, int>(71, 12, 0), 
                new Tuple<int, int, int>(29, 12, 90), 
                new Tuple<int, int, int>(116, 12, 180), 
                new Tuple<int, int, int>(209, 12, 270), 
                new Tuple<int, int, int>(85, 13, 0), // F R B L

                // 3 horizontal adjacent
                new Tuple<int, int, int>(69, 9, 0), // L F R
                new Tuple<int, int, int>(21, 9, 90), // F R B
                new Tuple<int, int, int>(84, 9, 180), // R B L
                new Tuple<int, int, int>(81, 9, 270), // B L F
                new Tuple<int, int, int>(7, 10, 0), // F FR R
                new Tuple<int, int, int>(28, 10, 90), // R BR B
                new Tuple<int, int, int>(112, 10, 180), // B BL L
                new Tuple<int, int, int>(193, 10, 270), // L FL F

                // 2 horizontal adjacent
                new Tuple<int, int, int>(5, 5, 0), // F R
                new Tuple<int, int, int>(20, 5, 90), // R B
                new Tuple<int, int, int>(80, 5, 180), // B L
                new Tuple<int, int, int>(65, 5, 270), // L F
                new Tuple<int, int, int>(273, 6, 0), // F B U
                new Tuple<int, int, int>(324, 6, 90), // L R U
                new Tuple<int, int, int>(17, 7, 0), // F B
                new Tuple<int, int, int>(68, 7, 90), // L R

                // 1 Horizontal adjacent
                new Tuple<int, int, int>(1, 3, 0), // F
                new Tuple<int, int, int>(4, 3, 90), // R
                new Tuple<int, int, int>(16, 3, 180), // B
                new Tuple<int, int, int>(64, 3, 270), // L

                // 0 horizontal adjacent
                new Tuple<int, int, int>(0, 1, 0), // None
            };
    }
}
