using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Version2
{
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileViewer : MonoBehaviour
{
    public TileInstance tile;
    public int shapeIndex;

    private void OnEnable()
    {
        if (tile != null)
        {
            Render();
        }
    }

    private void OnValidate()
    {
        if (tile != null)
        {
            Render();
        }
    }

    private void Render()
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
        int vertexIndex = 0;

        for (int s = 0; s < tile.shapes[shapeIndex].sides.Length; ++s)
        {
            Side side = tile.shapes[shapeIndex].sides[s];
            for (int f = 0; f < side.faces.Length; ++f)
            {
                Face face = side.faces[f];

                foreach (Vertices vertex in face.vertices)
                {
                    vertices.Add(vertex.position);
                    normals.Add(face.normal);
                    uvs.Add(vertex.uv);
                }
                
                foreach (int triangle in face.triangles)
                {
                    triangles[face.submesh].Add(vertexIndex + triangle);
                }

                vertexIndex += face.vertices.Length;
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

        meshFilter.sharedMesh = mesh;
    }
}
}