using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private Dictionary<Vector3Int, TileInfo> tiles;
    private TileInstance tile;

    public int Size => tiles.Count;

    private void Awake()
    {
        tiles = new Dictionary<Vector3Int, TileInfo>();
    }

    public void Init(TileInstance tile)
    {
        this.tile = tile;
    }

    public void AddTile(Vector3Int pos, int shapeIndex, int rotation)
    {
        tiles.Add(pos, new TileInfo(shapeIndex, rotation));
    }

    public bool ContainsTile(Vector3Int pos)
    {
        return tiles.ContainsKey(pos);
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

        foreach (var pair in tiles)
        {
            for (int s = 0; s < tile.shapes[pair.Value.shapeIndex].sides.Length; ++s)
            {
                Side side = tile.shapes[pair.Value.shapeIndex].sides[s];

                if (!RenderSide(pair.Key, s, RotateVector(side.neighbourDirection, tiles[pair.Key].rotation))) { continue; }

                for (int f = 0; f < side.faces.Length; ++f)
                {
                    Face face = side.faces[f];

                    foreach (Vertices vertex in face.vertices)
                    {
                        Vector3 centre = new Vector3(0.5f, 0.5f, 0.5f);
                        Vector3 direction = vertex.position - centre;
                        direction = Quaternion.Euler(0, tiles[pair.Key].rotation, 0) * direction;
                        vertices.Add(pair.Key + (direction + centre));
                        normals.Add(Quaternion.Euler(0, tiles[pair.Key].rotation, 0) * face.normal);
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
        if (tiles.ContainsKey(neighbourPos)) { return CompareFaces(tile.shapes[tiles[current].shapeIndex], tiles[current].rotation, side, tile.shapes[tiles[neighbourPos].shapeIndex], tiles[neighbourPos].rotation); }

        Vector3 rayDir = (neighbourPos + new Vector3(0.5f, 0.5f, 0.5f)) - (current + new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(current + new Vector3(0.5f, 0.5f, 0.5f), rayDir, out RaycastHit hit, 1.01f))
        {
            Segment other = hit.collider.gameObject.GetComponent<Segment>();
            if (!other.tiles.ContainsKey(neighbourPos)) { return true; }
            return CompareFaces(tile.shapes[tiles[current].shapeIndex], tiles[current].rotation, side, other.tile.shapes[other.tiles[neighbourPos].shapeIndex], other.tiles[neighbourPos].rotation);
        }

        return true;
    }

    private bool CompareFaces(TileShape currentShape, int currentRotation, int currentSide, TileShape neighbourShape, int neighbourRotation)
    {   
        int neighbourSide = GetSide(RotateVector(-RotateVector(currentShape.sides[currentSide].neighbourDirection, currentRotation), neighbourRotation + 360));
        bool sameSide = currentShape.sides[currentSide].sideType == neighbourShape.sides[neighbourSide].sideType;
        if (!sameSide) { return true; }
        return FaceMatchCheck(currentShape.sides[currentSide].sideType, currentRotation, neighbourRotation);
    }

    private int GetSide(Vector3Int direction)
    {
        if (direction == Vector3Int.back) { return 0; }
        if (direction == Vector3Int.forward) { return 1; }
        if (direction == Vector3Int.left) { return 2; }
        if (direction == Vector3Int.right) { return 3; }
        if (direction == Vector3Int.down) { return 4; }
        if (direction == Vector3Int.up) { return 5; }
        Debug.LogError("Direction is not along cardinal axes, direction: " + direction);
        return -1;
    }

    private Vector3Int RotateVector(Vector3Int start, int degree)
    {
        switch (degree)
        {
            case 0:
            case 360:
            case -360:
                return start;
            case 90:
            case -270:
            case 450:
                return new Vector3Int(start.z, start.y, -start.x);
            case 180:
            case -180:
            case 540:
            case -540:
                return new Vector3Int(-start.x, start.y, -start.z);
            case -90:
            case 270:
            case -450:
                return new Vector3Int(-start.z, start.y, start.x);
            default:
                Debug.LogError("Vector3Int was rotated off axes, angle: " + degree);
                return Vector3Int.zero;
        }
    }

    private bool FaceMatchCheck(SideType sideType, int currentRotation, int neighbourRotation)
    {
        bool sameRotation = currentRotation == neighbourRotation;
        switch (sideType)
        {
            case SideType.Cube:
                return false;
            case SideType.StairSide:
            case SideType.StairFront:
                return !sameRotation;
            default:
                return false;
        }
    }
}

[Serializable]
public class TileInfo
{
    public int shapeIndex;
    public int rotation;

    public TileInfo(int shapeIndex, int rotation)
    {
        this.shapeIndex = shapeIndex;
        this.rotation = rotation;
    }
}