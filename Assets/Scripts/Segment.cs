using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private Dictionary<Vector3Int, TileInfo> tiles;
    private TileInstance tile;
    private WorldBuilder worldBuilder;
    private Vector2Int dimensions;

    public int Size => tiles.Count;

    private void Awake()
    {
        tiles = new Dictionary<Vector3Int, TileInfo>();
    }

    public void Init(TileInstance tile, WorldBuilder worldBuilder, Vector2Int dimensions)
    {
        this.tile = tile;
        this.worldBuilder = worldBuilder;
        this.dimensions = dimensions;
    }

    public void AddTile(Vector3Int pos)
    {
        switch (tile.tileType)
        {
            case SmartShape.Basic:
                tiles.Add(pos, new TileInfo(0, 0));
                break;
            case SmartShape.Line:
                AddLineTile(pos, false);
                break;
            case SmartShape.Rect:
                AddRectTile(pos, false);
                break;
            case SmartShape.SmartLine:
                AddLineTile(pos, true);
                break;
            case SmartShape.SmartRect:
                AddRectTile(pos, true);
                break;
            case SmartShape.DirectionalLine:
                AddLineTile(pos, false);
                break;
            default:
                tiles.Add(pos, new TileInfo(0, 0));
                break;
        }
        
    }

    public bool ContainsTile(Vector3Int pos)
    {
        return tiles.ContainsKey(pos);
    }

    public void RemoveTile(Vector3Int pos)
    {
        tiles.Remove(pos);
    }

    private void AddLineTile(Vector3Int pos, bool smart)
    {
        byte sides = 0;
        sides += (byte)(GetNeighbour(pos + new Vector3Int(0, 0, 1), smart) > 0 ? 1 : 0); // top 
        sides += (byte)(GetNeighbour(pos + new Vector3Int(1, 0, 0), smart) > 0 ? 1 << 1 : 0); // right 
        sides += (byte)(GetNeighbour(pos + new Vector3Int(0, 0, -1), smart) > 0 ? 1 << 2 : 0); // bottom 
        sides += (byte)(GetNeighbour(pos + new Vector3Int(-1, 0, 0), smart) > 0 ? 1 << 3 : 0); // left 

        if ((sides & 5) == 5)
        {
            tiles.Add(pos, new TileInfo(0, -90));
        }
        else if ((sides & 10) == 10)
        {
            tiles.Add(pos, new TileInfo(0, 0));
        }
        else if ((sides & 1) == 1)
        {
            tiles.Add(pos, new TileInfo(1, -90));
        }
        else if ((sides & 2) == 2)
        {
            tiles.Add(pos, new TileInfo(1, 0));
        }
        else if ((sides & 4) == 4)
        {
            tiles.Add(pos, new TileInfo(2, -90));
        }
        else if ((sides & 8) == 8)
        {
            tiles.Add(pos, new TileInfo(2, 0));
        }
        else
        {
            tiles.Add(pos, new TileInfo(0, 0));
        }

        if ((sides & 1) == 1)
        {
            UpdateLineTile(pos + new Vector3Int(0, 0, 1), smart);
        }
        if ((sides & 2) == 2)
        {
            UpdateLineTile(pos + new Vector3Int(1, 0, 0), smart);
        }
        if ((sides & 4) == 4)
        {
            UpdateLineTile(pos + new Vector3Int(0, 0, -1), smart);
        }
        if ((sides & 8) == 8)
        {
            UpdateLineTile(pos + new Vector3Int(-1, 0, 0), smart);
        }
    }
    
    private void UpdateLineTile(Vector3Int pos, bool smart)
    {
        if (smart && !tiles.ContainsKey(pos)) 
        { 
            for (int i = 0; i < worldBuilder.tiles.Length; ++i)
            {
                if (worldBuilder.tiles[i].tileType == tile.tileType && worldBuilder.transform.GetChild(i).GetComponent<Segment>().ContainsTile(pos))
                {
                    worldBuilder.transform.GetChild(i).GetComponent<Segment>().UpdateRectTile(pos, smart);
                    break;
                }
            } 
        }
        else
        {
            byte sides = 0;
            sides += (byte)(GetNeighbour(pos + new Vector3Int(0, 0, 1), smart) > 0 ? 1 : 0); // top 
            sides += (byte)(GetNeighbour(pos + new Vector3Int(1, 0, 0), smart) > 0 ? 1 << 1 : 0); // right 
            sides += (byte)(GetNeighbour(pos + new Vector3Int(0, 0, -1), smart) > 0 ? 1 << 2 : 0); // bottom 
            sides += (byte)(GetNeighbour(pos + new Vector3Int(-1, 0, 0), smart) > 0 ? 1 << 3 : 0); // left 

            if ((sides & 5) == 5)
            {
                tiles[pos] = new TileInfo(0, -90);
            }
            else if ((sides & 10) == 10)
            {
                tiles[pos] = new TileInfo(0, 0);
            }
            else if ((sides & 1) == 1)
            {
                tiles[pos] = new TileInfo(1, -90);
            }
            else if ((sides & 2) == 2)
            {
                tiles[pos] = new TileInfo(1, 0);
            }
            else if ((sides & 4) == 4)
            {
                tiles[pos] = new TileInfo(2, -90);
            }
            else if ((sides & 8) == 8)
            {
                tiles[pos] = new TileInfo(2, 0);
            }
            else
            {
                tiles[pos] = new TileInfo(0, 0);
            }
        }
    }

    private void AddRectTile(Vector3Int pos, bool smart)
    {
        int sides = 0;
        sides += GetNeighbour(pos + new Vector3Int(0, 0, 1), smart) > 0 ? 1 : 0; // top 
        sides += GetNeighbour(pos + new Vector3Int(1, 0, 1), smart) > 0 ? 1 << 1 : 0; // top right 
        sides += GetNeighbour(pos + new Vector3Int(1, 0, 0), smart) > 0 ? 1 << 2 : 0; // right
        sides += GetNeighbour(pos + new Vector3Int(1, 0, -1), smart) > 0 ? 1 << 3 : 0; // bottom right 
        sides += GetNeighbour(pos + new Vector3Int(0, 0, -1), smart) > 0 ? 1 << 4 : 0; // bottom
        sides += GetNeighbour(pos + new Vector3Int(-1, 0, -1), smart) > 0 ? 1 << 5 : 0; // bottom left
        sides += GetNeighbour(pos + new Vector3Int(-1, 0, 0), smart) > 0 ? 1 << 6 : 0; // left
        sides += GetNeighbour(pos + new Vector3Int(-1, 0, 1), smart) > 0 ? 1 << 7 : 0; // top left 
        sides += GetNeighbour(pos + new Vector3Int(0, 1, 0), smart) > 0 ? 1 << 8 : 0; // above
        sides += GetNeighbour(pos + new Vector3Int(0, -1, 0), smart) > 0 ? 1 << 9 : 0; // below 

        if ((sides & 256) == 256)
        {
            tiles.Add(pos, new TileInfo(0, 0));
        }
        else if ((sides & 255) == 255)
        {
            tiles.Add(pos, new TileInfo(0, 0));
        }
        else if ((sides & 253) == 253)
        {
            tiles.Add(pos, new TileInfo(3, 180));
        }
        else if ((sides & 247) == 247)
        {
            tiles.Add(pos, new TileInfo(3, -90));
        }
        else if ((sides & 223) == 223)
        {
            tiles.Add(pos, new TileInfo(3, 0));
        }
        else if ((sides & 127) == 127)
        {
            tiles.Add(pos, new TileInfo(3, 90));
        }
        else if ((sides & 84) == 84)
        {
            tiles.Add(pos, new TileInfo(1, 180));
        }
        else if ((sides & 81) == 81)
        {
            tiles.Add(pos, new TileInfo(1, -90));
        }
        else if ((sides & 69) == 69)
        {
            tiles.Add(pos, new TileInfo(1, 0));
        }
        else if ((sides & 21) == 21)
        {
            tiles.Add(pos, new TileInfo(1, 90));
        }
        else if ((sides & 80) == 80)
        {
            tiles.Add(pos, new TileInfo(2, 180));
        }
        else if ((sides & 65) == 65)
        {
            tiles.Add(pos, new TileInfo(2, -90));
        }
        else if ((sides & 5) == 5)
        {
            tiles.Add(pos, new TileInfo(2, 0));
        }
        else if ((sides & 20) == 20)
        {
            tiles.Add(pos, new TileInfo(2, 90));
        }
        else 
        {
            tiles.Add(pos, new TileInfo(0, 0));
        }

        if ((sides & 1) == 1)
        {
            UpdateRectTile(pos + new Vector3Int(0, 0, 1), smart);
        }
        if ((sides & 2) == 2)
        {
            UpdateRectTile(pos + new Vector3Int(1, 0, 1), smart);
        }
        if ((sides & 4) == 4)
        {
            UpdateRectTile(pos + new Vector3Int(1, 0, 0), smart);
        }
        if ((sides & 8) == 8)
        {
            UpdateRectTile(pos + new Vector3Int(1, 0, -1), smart);
        }
        if ((sides & 16) == 16)
        {
            UpdateRectTile(pos + new Vector3Int(0, 0, -1), smart);
        }
        if ((sides & 32) == 32)
        {
            UpdateRectTile(pos + new Vector3Int(-1, 0, -1), smart);
        }
        if ((sides & 64) == 64)
        {
            UpdateRectTile(pos + new Vector3Int(-1, 0, 0), smart);
        }
        if ((sides & 128) == 128)
        {
            UpdateRectTile(pos + new Vector3Int(-1, 0, 1), smart);
        }
        if ((sides & 512) == 512)
        {
            UpdateRectTile(pos + new Vector3Int(0, -1, 0), smart);
        }
    }

    private void UpdateRectTile(Vector3Int pos, bool smart)
    {
        if (smart && !tiles.ContainsKey(pos)) 
        { 
            for (int i = 0; i < worldBuilder.tiles.Length; ++i)
            {
                if (worldBuilder.tiles[i].tileType == tile.tileType && worldBuilder.transform.GetChild(i).GetComponent<Segment>().ContainsTile(pos))
                {
                    worldBuilder.transform.GetChild(i).GetComponent<Segment>().UpdateRectTile(pos, smart);
                    break;
                }
            } 
        }
        else
        {
            int sides = 0;
            sides += GetNeighbour(pos + new Vector3Int(0, 0, 1), smart) > 0 ? 1 : 0; // top 
            sides += GetNeighbour(pos + new Vector3Int(1, 0, 1), smart) > 0 ? 1 << 1 : 0; // top right 
            sides += GetNeighbour(pos + new Vector3Int(1, 0, 0), smart) > 0 ? 1 << 2 : 0; // right
            sides += GetNeighbour(pos + new Vector3Int(1, 0, -1), smart) > 0 ? 1 << 3 : 0; // bottom right 
            sides += GetNeighbour(pos + new Vector3Int(0, 0, -1), smart) > 0 ? 1 << 4 : 0; // bottom
            sides += GetNeighbour(pos + new Vector3Int(-1, 0, -1), smart) > 0 ? 1 << 5 : 0; // bottom left
            sides += GetNeighbour(pos + new Vector3Int(-1, 0, 0), smart) > 0 ? 1 << 6 : 0; // left
            sides += GetNeighbour(pos + new Vector3Int(-1, 0, 1), smart) > 0 ? 1 << 7 : 0; // top left 
            sides += GetNeighbour(pos + new Vector3Int(0, 1, 0), smart) > 0 ? 1 << 8 : 0; // above
            sides += GetNeighbour(pos + new Vector3Int(0, -1, 0), smart) > 0 ? 1 << 9 : 0; // below 

            if ((sides & 256) == 256)
            {
                tiles[pos] = new TileInfo(0, 0);
            }
            else if ((sides & 255) == 255)
            {
                tiles[pos] = new TileInfo(0, 0);
            }
            else if ((sides & 253) == 253)
            {
                tiles[pos] = new TileInfo(3, 180);
            }
            else if ((sides & 247) == 247)
            {
                tiles[pos] = new TileInfo(3, -90);
            }
            else if ((sides & 223) == 223)
            {
                tiles[pos] = new TileInfo(3, 0);
            }
            else if ((sides & 127) == 127)
            {
                tiles[pos] = new TileInfo(3, 90);
            }
            else if ((sides & 84) == 84)
            {
                tiles[pos] = new TileInfo(1, 180);
            }
            else if ((sides & 81) == 81)
            {
                tiles[pos] = new TileInfo(1, -90);
            }
            else if ((sides & 69) == 69)
            {
                tiles[pos] = new TileInfo(1, 0);
            }
            else if ((sides & 21) == 21)
            {
                tiles[pos] = new TileInfo(1, 90);
            }
            else if ((sides & 80) == 80)
            {
                tiles[pos] = new TileInfo(2, 180);
            }
            else if ((sides & 65) == 65)
            {
                tiles[pos] = new TileInfo(2, -90);
            }
            else if ((sides & 5) == 5)
            {
                tiles[pos] = new TileInfo(2, 0);
            }
            else if ((sides & 20) == 20)
            {
                tiles[pos] = new TileInfo(2, 90);
            }
            else 
            {
                tiles[pos] = new TileInfo(0, 0);
            }
        }
    }

    private int GetNeighbour(Vector3Int pos, bool smart)
    {
        if (pos.x < 0 || pos.x >= dimensions.x || pos.z < 0 || pos.z >= dimensions.y || pos.y < 0 || pos.y > 16) { return 3; }
        if (tiles.ContainsKey(pos)) { return 1; }
        if (!smart) { return 0; }
        for (int i = 0; i < worldBuilder.tiles.Length; ++i)
        {
            if (worldBuilder.tiles[i].tileType == tile.tileType && worldBuilder.transform.GetChild(i).GetComponent<Segment>().ContainsTile(pos))
            {
                return 2;
            }
        }
        return 0;
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
            int rotation = tiles[pair.Key].rotation;
            for (int s = 0; s < tile.shapes[pair.Value.shapeIndex].sides.Length; ++s)
            {
                Side side = tile.shapes[pair.Value.shapeIndex].sides[s];

                if (!RenderSide(pair.Key, s, RotateVector(side.neighbourDirection, rotation))) { continue; }

                for (int f = 0; f < side.faces.Length; ++f)
                {
                    Face face = side.faces[f];

                    foreach (Vertices vertex in face.vertices)
                    {
                        Vector3 centre = new Vector3(0.5f, 0.5f, 0.5f);
                        Vector3 direction = vertex.position - centre;
                        direction = Quaternion.Euler(0, rotation, 0) * direction;
                        Vector3 v = pair.Key + (direction + centre);
                        vertices.Add(v);

                        Vector3 n = Quaternion.Euler(0, rotation, 0) * face.normal;
                        normals.Add(n);

                        Vector2 u;
                        if (tile.rotateXZface && (s == 4 || s == 5))
                        {
                            Vector3 uvDirection = new Vector3(vertex.uv.x, vertex.uv.y, 0) - centre;
                            uvDirection = Quaternion.Euler(0, 0, rotation) * uvDirection;
                            u = new Vector2(uvDirection.x, uvDirection.y);
                        }
                        else
                        {
                            u = vertex.uv;
                        }
                        uvs.Add(u);
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
        
        for (int i = 0; i < worldBuilder.tiles.Length; ++i)
        {
            Segment segment = worldBuilder.transform.GetChild(i).GetComponent<Segment>();
            if (segment.ContainsTile(neighbourPos))
            {
                return CompareFaces(tile.shapes[tiles[current].shapeIndex], tiles[current].rotation, side, segment.tile.shapes[segment.tiles[neighbourPos].shapeIndex], segment.tiles[neighbourPos].rotation);
            }
        }

        return true;
    }

    private bool CompareFaces(TileShape currentShape, int currentRotation, int currentSide, TileShape neighbourShape, int neighbourRotation)
    {   
        int neighbourSide = GetSide(RotateVector(-RotateVector(currentShape.sides[currentSide].neighbourDirection, currentRotation), neighbourRotation + 360));
        if (currentShape.sides[currentSide].sideType == SideType.CliffSide && neighbourShape.sides[neighbourSide].sideType == SideType.CliffSideSlope) { return false; }
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
            case SideType.CliffSide:
                return false;
            case SideType.StairSide:
            case SideType.StairFront:
            case SideType.CliffSideSlope:
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