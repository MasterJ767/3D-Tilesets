using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private List<Vector3Int> tiles;
    private TileInstance tile;

    public int Size => tiles.Count;

    private void Awake()
    {
        tiles = new List<Vector3Int>();
    }

    public void Init(TileInstance tile)
    {
        this.tile = tile;
    }

    public void AddTile(Vector3Int pos)
    {
        tiles.Add(pos);
    }

    public bool ContainsTile(Vector3Int pos)
    {
        return tiles.Contains(pos);
    }

    public void RemoveTile(Vector3Int pos)
    {
        tiles.Remove(pos);
    }

    public void Render()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int>[] triangles = new List<int>[3];
        triangles[0] = new List<int>();
        triangles[1] = new List<int>();
        triangles[2] = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int vertexIndex = 0;

        foreach (Vector3Int pos in tiles)
        {
            for (int s = 0; s < tile.shape.sides.Length; ++s)
            {
                Side side = tile.shape.sides[s];

                Vector3Int neighbourPos = pos + side.neighbourDirection;
                if (!RenderSide(pos, s, side.neighbourDirection)) { continue; }

                for (int f = 0; f < side.faces.Length; ++f)
                {
                    Face face = side.faces[f];

                    foreach (Vertices vertex in face.vertices)
                    {
                        vertices.Add(pos + vertex.position);
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
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private bool RenderSide(Vector3Int current, int side, Vector3Int neighbourDirection)
    {
        Vector3Int neighbourPos = current + neighbourDirection;
        if (neighbourPos.y < 0) { return false; }
        if (tiles.Contains(neighbourPos)) { return tile.shape.sides[side].sideType != tile.shape.sides[GetOppositeSide(side)].sideType; }

        Vector3 rayDir = (neighbourPos + new Vector3(0.5f, 0.5f, 0.5f)) - (current + new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(current + new Vector3(0.5f, 0.5f, 0.5f), rayDir, out RaycastHit hit, 1.01f))
        {
            Debug.Log(hit.collider.gameObject.name);
            Segment other = hit.collider.gameObject.GetComponent<Segment>();
            if (!other.tiles.Contains(neighbourPos)) { return true; }
            return tile.shape.sides[side].sideType != other.tile.shape.sides[GetOppositeSide(side)].sideType;
        }

        return true;
    }

    private int GetOppositeSide(int side)
    {
        return (side & 1) == 0 ? side + 1 : side - 1;
    }
}
