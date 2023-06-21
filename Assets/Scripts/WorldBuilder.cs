using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public Vector2Int baseDimensions;
    public TileInstance[] tiles;
    
    //private static readonly int segmentLimit = 4096;
    private List<Segment> segments;

    private void Start()
    {
        GameObject newSegment = new GameObject(tiles[0].tileName);
        newSegment.transform.position = new Vector3(0, 0, 0);
        newSegment.transform.parent = transform;

        Segment segment = newSegment.AddComponent<Segment>();
        newSegment.AddComponent<MeshFilter>();
        MeshRenderer segmentRenderer = newSegment.AddComponent<MeshRenderer>();
        newSegment.AddComponent<MeshCollider>();

        segment.Init(tiles[0]);
        
        for (int i = 0; i < baseDimensions.x; ++i)
        {
            for (int j = 0; j < baseDimensions.y; ++j)
            {
                segment.AddTile(new Vector3Int(i, 0, j));
            }
        }
        segment.AddTile(new Vector3Int(0, 2, 0));

        segment.Render();
    }
}
