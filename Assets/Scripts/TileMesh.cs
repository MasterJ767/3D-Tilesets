using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version3
{
    public class TileMesh : MonoBehaviour
    {
        private TileData tile;
        private Chunk chunk;
        private int width;
        private int height;
        private bool[,,] tileMap;

        public void Initialise(TileData tile, Chunk chunk, int width, int height)
        {
            this.tile = tile;
            this.chunk = chunk;
            this.width = width;
            this.height = height;
            tileMap = new bool[width, height, width];
        }

        public void AddTile(Vector3Int pos)
        {
            tileMap[pos.x, pos.y, pos.z] = true;
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
                        bool current = tileMap[x, y, z];
                        if (current == false) { continue; }
                        Mesh instance = tile.meshes[0];

                        instance.GetVertices(instanceVertices);
                        for (int v = 0; v < instanceVertices.Count; ++v)
                        {
                            instanceVertices[v] += new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                        }
                        vertices.AddRange(instanceVertices);

                        instance.GetNormals(instanceNormals);
                        normals.AddRange(instanceNormals);

                        for (int i = 0; i < instance.subMeshCount; ++i) {
                            instance.GetTriangles(instanceTriangles[i], i);
                            for (int t = 0; t < instanceTriangles[i].Count; ++t)
                            {
                                instanceTriangles[i][t] += vertexCount;
                            }
                            triangles[i].AddRange(instanceTriangles[i]);
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
    }
}
