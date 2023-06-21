using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldBuilder : MonoBehaviour
{
    public Vector2Int baseDimensions;
    public TileInstance[] tiles;
    public BuilderCursor cursor;
    
    //private static readonly int segmentLimit = 4096;
    private List<Segment> segments;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Camera mainCamera;
    private bool buildMode = true;
    private int activeTile = 0;

    private void Awake()
    {
        segments = new List<Segment>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = cursor.buildMaterial;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        Segment segment = CreateSegment(tiles[0]);
        
        for (int i = 0; i < baseDimensions.x; ++i)
        {
            for (int j = 0; j < baseDimensions.y; ++j)
            {
                segment.AddTile(new Vector3Int(i, 0, j));
            }
        }
        segment.AddTile(new Vector3Int(0, 1, 5));
        segment.AddTile(new Vector3Int(4, 1, 5));
        segment.AddTile(new Vector3Int(5, 1, 5));
        segment.AddTile(new Vector3Int(6, 1, 5));
        segment.AddTile(new Vector3Int(7, 1, 5));
        for (int k = 0; k < 15; ++k)
        {
            for (int l = 6; l < 22; ++l)
            {
                segment.AddTile(new Vector3Int(k, 1, l));
            }
        }

        for (int m = 0; m < 10; ++m)
        {
            for (int n = 11; n < 19; ++n)
            {
                segment.AddTile(new Vector3Int(m, 2, n));
            }
        }

        Segment segment2 = CreateSegment(tiles[1]);
        segment2.AddTile(new Vector3Int(1, 1, 5));
        segment2.AddTile(new Vector3Int(2, 1, 5));
        segment2.AddTile(new Vector3Int(3, 1, 5));

        RenderSegments();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) { SwapMode(); }

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Vector3Int hitPos = new Vector3Int(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z));
            Vector3Int placePos;
            Segment segment = hit.collider.gameObject.GetComponent<Segment>();
            bool found = segment.ContainsTile(hitPos);
            if ((found && buildMode) || (!found && !buildMode))
            {
                Vector3 direction = (hit.point - (hitPos + new Vector3(0.5f, 0.5f, 0.5f))).normalized;
                float max = Mathf.Max(Mathf.Abs(direction.x), Mathf.Max(Mathf.Abs(direction.y), Mathf.Abs(direction.z)));
                Vector3Int normalized = new Vector3Int(Mathf.RoundToInt(direction.x / max), Mathf.RoundToInt(direction.y / max), Mathf.RoundToInt(direction.z / max));
                normalized.y = normalized.y == 1 && (normalized.y == normalized.x || normalized.y == normalized.z) ? 0 : normalized.y;
                normalized.z = normalized.z == 1 && (normalized.z == normalized.x) ? 0 : normalized.z;
                placePos = hitPos + normalized;
            }
            else
            {
                placePos = hitPos;
            }

            if (placePos.y < 0 || (!buildMode && !segment.ContainsTile(placePos)))
            {
                meshFilter.mesh = new Mesh();
            }
            else
            {
                TileShape shape = buildMode ? tiles[activeTile].shape : cursor.cursorShape;
                RenderCursor(shape, placePos, buildMode ? 1.0f : 1.1f);
                if (Input.GetMouseButtonDown(0)) 
                {
                    if (buildMode) { AddTileToWorld(placePos); }
                    else { RemoveTileFromWorld(segment, placePos); }
                }
            }            
        }
        else
        {
            meshFilter.mesh = new Mesh();
        }
    }

    private Segment CreateSegment(TileInstance tile)
    {
        GameObject newSegment = new GameObject(tile.tileName);
        newSegment.transform.position = new Vector3(0, 0, 0);
        newSegment.transform.parent = transform;

        Segment segment = newSegment.AddComponent<Segment>();
        newSegment.AddComponent<MeshFilter>();
        MeshRenderer segmentRenderer = newSegment.AddComponent<MeshRenderer>();
        newSegment.AddComponent<MeshCollider>();

        segment.Init(tile);
        segments.Add(segment);

        return segment;
    }

    private void RenderSegments()
    {
        foreach (Segment segment in segments)
        {
            segment.Render();
        }
    }

    private void SwapMode()
    {
        buildMode = !buildMode;
        meshRenderer.material = buildMode ? cursor.buildMaterial : cursor.destroyMaterial;
    }

    private void RenderCursor(TileShape shape, Vector3Int pos, float size = 1)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertexIndex = 0;
        float offset = 0.5f - (size * 0.5f);

        for (int s = 0; s < shape.sides.Length; ++s)
        {
            Side side = shape.sides[s];

            for (int f = 0; f < side.faces.Length; ++f)
            {
                Face face = side.faces[f];

                foreach (Vertices vertex in face.vertices)
                {
                    vertices.Add(pos + (vertex.position * size) + new Vector3(offset, offset, offset));
                }
                
                foreach (int triangle in face.triangles)
                {
                    triangles.Add(vertexIndex + triangle);
                }

                vertexIndex += face.vertices.Length;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Cursor";
        mesh.SetVertices(vertices.ToArray());
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.Optimize();

        meshFilter.mesh = mesh;
    }

    private void AddTileToWorld(Vector3Int pos)
    {
        Segment segment = transform.Find(tiles[activeTile].tileName).GetComponent<Segment>();
        segment.AddTile(pos);
        segment.Render();
    }

    private void RemoveTileFromWorld(Segment segment, Vector3Int pos)
    {
        segment.RemoveTile(pos);
        segment.Render();
    }
}

[Serializable]
public class BuilderCursor
{
    public Material buildMaterial;
    public Material destroyMaterial;
    public TileShape cursorShape;
}