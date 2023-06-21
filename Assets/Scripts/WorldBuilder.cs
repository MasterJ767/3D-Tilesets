using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldBuilder : MonoBehaviour
{
    public Vector2Int baseDimensions;
    public TileInstance[] tiles;
    
    //private static readonly int segmentLimit = 4096;
    private List<Segment> segments;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        segments = new List<Segment>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
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
}
