using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBuilder : MonoBehaviour
{
    public Tile[] tiles;
    Dictionary<Vector3Int,(int, GameObject)> tileMap = new Dictionary<Vector3Int, (int, GameObject)>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Place();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Remove();
        }
    }

    private void Place()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (tileMap.Keys.Count == 0)
        {
            AddTile(new Vector3Int(0, 0, 0), 0);
        }
        else if (Physics.Raycast(inputRay, out hit))
        {
            Vector3 pos = hit.collider.transform.position;
            Vector3 direciton = hit.point - pos;
            float max = Mathf.Max(Mathf.Abs(direciton.x), Mathf.Max(Mathf.Abs(direciton.y), Mathf.Abs(direciton.z)));
            Vector3Int normalized = new Vector3Int(Mathf.RoundToInt(direciton.x / max), Mathf.RoundToInt(direciton.y / max), Mathf.RoundToInt(direciton.z / max));
            Vector3Int dest = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)) + normalized;
            if (!tileMap.ContainsKey(dest))
            {
                AddTile(dest, 0);
            }
        }
    }

    private void AddTile(Vector3Int dest, int tileIndex)
    {
        byte sides = 0;
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 0, 1)) ? 1 : 0); // top 
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, 1)) ? 1 << 1 : 0); // top right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, 0)) ? 1 << 2 : 0); // right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, -1)) ? 1 << 3 : 0); // bottom right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 0, -1)) ? 1 << 4 : 0); // bottom
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, -1)) ? 1 << 5 : 0); // bottom left
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 0)) ? 1 << 6 : 0); // left
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 1)) ? 1 << 7 : 0); // top left
        byte sides2 = 0;
        sides2 += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 1, 0)) ? 1 : 0); // above
        sides2 += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, -1, 0)) ? 1 << 1 : 0); // below
        if ((sides2 & 1) == 1)
        {
            if (sides == 0) // not neighboured
            {
                AddMapEntry(dest, tileIndex, 15);
            }

            // neighboured on 3 sides
            else if ((sides & 84) == 84) // neighboured on right, bottom and left
            {
                AddMapEntry(dest, tileIndex, 16);
            }
            else if ((sides & 81) == 81) // neighboured on top, left and bottom
            {
                AddMapEntry(dest, tileIndex, 18);
            }
            else if ((sides & 69) == 69) // neighboured on right, top and left
            {
                AddMapEntry(dest, tileIndex, 20);
            }
            else if ((sides & 21) == 21) // neighboured on bottom, right and top
            {
                AddMapEntry(dest, tileIndex, 22);
            }

            //neighboured on 2 sides
            else if ((sides & 80) == 80) // neighboured on bottom and left
            {
                AddMapEntry(dest, tileIndex, 17);
            }
            else if ((sides & 65) == 65) // neighboured on top and left
            {
                AddMapEntry(dest, tileIndex, 19);
            }
            else if ((sides & 5) == 5) // neighboured on right and top
            {
                AddMapEntry(dest, tileIndex, 21);
            }
            else if ((sides & 20) == 20) // neighboured on bottom and right
            {
                AddMapEntry(dest, tileIndex, 23);
            }
            else if ((sides & 17) == 17) // neighboured on top and bottom
            {
                AddMapEntry(dest, tileIndex, 25);
            }
            else if ((sides & 68) == 68) // neighboured on left and right
            {
                AddMapEntry(dest, tileIndex, 28);
            }

            // neighboured on 1 side
            else if ((sides & 16) == 16) // neighboured on bottom
            {
                AddMapEntry(dest, tileIndex, 24);
            }
            else if ((sides & 1) == 1) // neighboured on top
            {
                AddMapEntry(dest, tileIndex, 26);
            }
            else if ((sides & 4) == 4) // neighboured on right
            {
                AddMapEntry(dest, tileIndex, 27);
            }
            else if ((sides & 64) == 64) // neighboured on left
            {
                AddMapEntry(dest, tileIndex, 29);
            }
        }
        else 
        {
            if (sides == 0) // no neighbours
            {
                AddMapEntry(dest, tileIndex, 0);
            }
            // neighboured on 4 sides
            // no corners
            else if ((sides & 255) == 255) //  neighboured on all 4 sides no cutouts
            {
                AddMapEntry(dest, tileIndex, 30);
            }
            // 1 corner
            // 2 corners
            // 3 corners
            // 4 corners
            else if ((sides & 85) == 85) // neighboured on all 4 sides
            {
                AddMapEntry(dest, tileIndex, 47);
            }

            // neighboured on 3 sides
            // no corners
            else if ((sides & 124) == 124) // neighboured on right, bottom and left no cutouts
            {
                AddMapEntry(dest, tileIndex, 1);
            }
            else if ((sides & 241) == 241) // neighboured on top, left and bottom no cutouts
            {
                AddMapEntry(dest, tileIndex, 3);
            }
            else if ((sides & 199) == 199) // neighboured on right, top and left no cutouts
            {
                AddMapEntry(dest, tileIndex, 5);
            }
            else if ((sides & 31) == 31) // neighboured on bottom, right and top no cutouts
            {
                AddMapEntry(dest, tileIndex, 7);
            }
            // 1 corners
            else if ((sides & 92) == 92)
            {
                AddMapEntry(dest, tileIndex, 35); // neighboured on right, bottom and left cutout on bottom left
            }
            else if ((sides & 116) == 116)
            {
                AddMapEntry(dest, tileIndex, 39); // neighboured on right, bottom and left cutout on bottom right
            }
            else if ((sides & 113) == 113)
            {
                AddMapEntry(dest, tileIndex, 43); // neighboured on top, left and bottom cutout on top left
            }
            else if ((sides & 209) == 209)
            {
                AddMapEntry(dest, tileIndex, 41); // neighboured on top, left and bottom cutout on bottom left
            }
            else if ((sides & 197) == 197)
            {
                AddMapEntry(dest, tileIndex, 38); // neighboured on right, top and left cutout on top right
            }
            else if ((sides & 71) == 71)
            {
                AddMapEntry(dest, tileIndex, 36); // neighboured on right, top and left cutout on top left
            }
            else if ((sides & 23) == 23)
            {
                AddMapEntry(dest, tileIndex, 46); // neighboured on bottom, right and top cutout on bottom right
            }
            else if ((sides & 29) == 29)
            {
                AddMapEntry(dest, tileIndex, 44); // neighboured on bottom, right and top cutout on top right
            }
            // 2 corners
            else if ((sides & 84) == 84) // neighboured on right, bottom and left
            {
                AddMapEntry(dest, tileIndex, 40);
            }
            else if ((sides & 81) == 81) // neighboured on top, left and bottom
            {
                AddMapEntry(dest, tileIndex, 42);
            }
            else if ((sides & 69) == 69) // neighboured on right, top and left
            {
                AddMapEntry(dest, tileIndex, 37);
            }
            else if ((sides & 21) == 21) // neighboured on bottom, right and top
            {
                AddMapEntry(dest, tileIndex, 45);
            }

            //neighboured on 2 sides
            // no corners
            else if ((sides & 112) == 112) // neighboured on bottom and left no bottom left cutout 
            {
                AddMapEntry(dest, tileIndex, 2);
            }
            else if ((sides & 193) == 193) // neighboured on top and left no top left cutout
            {
                AddMapEntry(dest, tileIndex, 4);
            }
            else if ((sides & 7) == 7) // neighboured on right and top no top right cutout
            {
                AddMapEntry(dest, tileIndex, 6);
            }
            else if ((sides & 28) == 28) // neighboured on bottom and right no bottom right cutout
            {
                AddMapEntry(dest, tileIndex, 8);
            }
            // 1 corner
            else if ((sides & 80) == 80) // neighboured on bottom and left
            {
                AddMapEntry(dest, tileIndex, 31);
            }
            else if ((sides & 65) == 65) // neighboured on top and left
            {
                AddMapEntry(dest, tileIndex, 32);
            }
            else if ((sides & 5) == 5) // neighboured on right and top
            {
                AddMapEntry(dest, tileIndex, 33);
            }
            else if ((sides & 20) == 20) // neighboured on bottom and right
            {
                AddMapEntry(dest, tileIndex, 34);
            }
            // opposite sides bordered
            else if ((sides & 17) == 17) // neighboured on top and bottom
            {
                AddMapEntry(dest, tileIndex, 10);
            }
            else if ((sides & 68) == 68) // neighboured on left and right
            {
                AddMapEntry(dest, tileIndex, 13);
            }

            //neighboured on 1 side
            else if ((sides & 16) == 16) // neighboured on bottom
            {
                AddMapEntry(dest, tileIndex, 9);
            }
            else if ((sides & 1) == 1) // neighboured on top
            {
                AddMapEntry(dest, tileIndex, 11);
            }
            else if ((sides & 4) == 4) // neighboured on right
            {
                AddMapEntry(dest, tileIndex, 12);
            }
            else if ((sides & 64) == 64) // neighboured on left
            {
                AddMapEntry(dest, tileIndex, 14);
            }
        }

        if ((sides2 & 2) == 2)
        {
            UpdateTile(dest + new Vector3Int(0, -1, 0));
        }
        if ((sides & 1) == 1)
        {
            UpdateTile(dest + new Vector3Int(0, 0, 1));
        }
        if ((sides & 2) == 2)
        {
            UpdateTile(dest + new Vector3Int(1, 0, 1));
        }
        if ((sides & 4) == 4)
        {
            UpdateTile(dest + new Vector3Int(1, 0, 0));
        }
        if ((sides & 8) == 8)
        {
            UpdateTile(dest + new Vector3Int(1, 0, -1));
        }
        if ((sides & 16) == 16)
        {
            UpdateTile(dest + new Vector3Int(0, 0, -1));
        }
        if ((sides & 32) == 32)
        {
            UpdateTile(dest + new Vector3Int(-1, 0, -1));
        }
        if ((sides & 64) == 64)
        {
            UpdateTile(dest + new Vector3Int(-1, 0, 0));
        }
        if ((sides & 128) == 128)
        {
            UpdateTile(dest + new Vector3Int(-1, 0, 1));
        }
    }

    private void UpdateTile(Vector3Int dest)
    {
        byte sides = 0;
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 0, 1)) ? 1 : 0); // top 
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, 1)) ? 1 << 1 : 0); // top right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, 0)) ? 1 << 2 : 0); // right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(1, 0, -1)) ? 1 << 3 : 0); // bottom right
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 0, -1)) ? 1 << 4 : 0); // bottom
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, -1)) ? 1 << 5 : 0); // bottom left
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 0)) ? 1 << 6 : 0); // left
        sides += (byte)(tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 1)) ? 1 << 7 : 0); // top left
        byte sides2 = 0;
        sides2 += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, 1, 0)) ? 1 : 0); // above
        sides2 += (byte)(tileMap.ContainsKey(dest + new Vector3Int(0, -1, 0)) ? 1 << 1 : 0); // below

        if ((sides2 & 1) == 1)
        {
            if (sides == 0) // not neighboured
            {
                ReplaceMapEntry(dest, 15);
            }
            else if ((sides & 85) == 85) // neighboured on all 4 sides
            {
                NullMapEntry(dest);
            }

            // neighboured on 3 sides
            else if ((sides & 84) == 84) // neighboured on right, bottom and left
            {
                ReplaceMapEntry(dest, 16);
            }
            else if ((sides & 81) == 81) // neighboured on top, left and bottom
            {
                ReplaceMapEntry(dest, 18);
            }
            else if ((sides & 69) == 69) // neighboured on right, top and left
            {
                ReplaceMapEntry(dest, 20);
            }
            else if ((sides & 21) == 21) // neighboured on bottom, right and top
            {
                ReplaceMapEntry(dest, 22);
            }

            //neighboured on 2 sides
            else if ((sides & 80) == 80) // neighboured on bottom and left
            {
                ReplaceMapEntry(dest, 17);
            }
            else if ((sides & 65) == 65) // neighboured on top and left
            {
                ReplaceMapEntry(dest, 19);
            }
            else if ((sides & 5) == 5) // neighboured on right and top
            {
                ReplaceMapEntry(dest, 21);
            }
            else if ((sides & 20) == 20) // neighboured on bottom and right
            {
                ReplaceMapEntry(dest, 23);
            }
            else if ((sides & 17) == 17) // neighboured on top and bottom
            {
                ReplaceMapEntry(dest, 25);
            }
            else if ((sides & 68) == 68) // neighboured on left and right
            {
                ReplaceMapEntry(dest, 28);
            }

            // neighboured on 1 side
            else if ((sides & 16) == 16) // neighboured on bottom
            {
                ReplaceMapEntry(dest, 24);
            }
            else if ((sides & 1) == 1) // neighboured on top
            {
                ReplaceMapEntry(dest, 26);
            }
            else if ((sides & 4) == 4) // neighboured on right
            {
                ReplaceMapEntry(dest, 27);
            }
            else if ((sides & 64) == 64) // neighboured on left
            {
                ReplaceMapEntry(dest, 29);
            }
        }
        else 
        {
            if (sides == 0) // no neighbours
            {
                ReplaceMapEntry(dest, 0);
            }
            // neighboured on 4 sides
            // no corners
            else if ((sides & 255) == 255) //  neighboured on all 4 sides no cutouts
            {
                ReplaceMapEntry(dest, 30);
            }
            // 1 corner
            // 2 corners
            // 3 corners
            // 4 corners
            else if ((sides & 85) == 85) // neighboured on all 4 sides
            {
                ReplaceMapEntry(dest, 47);
            }

            // neighboured on 3 sides
            // no corners
            else if ((sides & 124) == 124) // neighboured on right, bottom and left no cutouts
            {
                ReplaceMapEntry(dest, 1);
            }
            else if ((sides & 241) == 241) // neighboured on top, left and bottom no cutouts
            {
                ReplaceMapEntry(dest, 3);
            }
            else if ((sides & 199) == 199) // neighboured on right, top and left no cutouts
            {
                ReplaceMapEntry(dest, 5);
            }
            else if ((sides & 31) == 31) // neighboured on bottom, right and top no cutouts
            {
                ReplaceMapEntry(dest, 7);
            }
            // 1 corners
            else if ((sides & 92) == 92)
            {
                ReplaceMapEntry(dest, 35); // neighboured on right, bottom and left cutout on bottom left
            }
            else if ((sides & 116) == 116)
            {
                ReplaceMapEntry(dest, 39); // neighboured on right, bottom and left cutout on bottom right
            }
            else if ((sides & 113) == 113)
            {
                ReplaceMapEntry(dest, 43); // neighboured on top, left and bottom cutout on top left
            }
            else if ((sides & 209) == 209)
            {
                ReplaceMapEntry(dest, 41); // neighboured on top, left and bottom cutout on bottom left
            }
            else if ((sides & 197) == 197)
            {
                ReplaceMapEntry(dest, 38); // neighboured on right, top and left cutout on top right
            }
            else if ((sides & 71) == 71)
            {
                ReplaceMapEntry(dest, 36); // neighboured on right, top and left cutout on top left
            }
            else if ((sides & 23) == 23)
            {
                ReplaceMapEntry(dest, 46); // neighboured on bottom, right and top cutout on bottom right
            }
            else if ((sides & 29) == 29)
            {
                ReplaceMapEntry(dest, 44); // neighboured on bottom, right and top cutout on top right
            }
            // 2 corners
            else if ((sides & 84) == 84) // neighboured on right, bottom and left
            {
                ReplaceMapEntry(dest, 40);
            }
            else if ((sides & 81) == 81) // neighboured on top, left and bottom
            {
                ReplaceMapEntry(dest, 42);
            }
            else if ((sides & 69) == 69) // neighboured on right, top and left
            {
                ReplaceMapEntry(dest, 37);
            }
            else if ((sides & 21) == 21) // neighboured on bottom, right and top
            {
                ReplaceMapEntry(dest, 45);
            }

            //neighboured on 2 sides
            // no corners
            else if ((sides & 112) == 112) // neighboured on bottom and left no bottom left cutout 
            {
                ReplaceMapEntry(dest, 2);
            }
            else if ((sides & 193) == 193) // neighboured on top and left no top left cutout
            {
                ReplaceMapEntry(dest, 4);
            }
            else if ((sides & 7) == 7) // neighboured on right and top no top right cutout
            {
                ReplaceMapEntry(dest, 6);
            }
            else if ((sides & 28) == 28) // neighboured on bottom and right no bottom right cutout
            {
                ReplaceMapEntry(dest, 8);
            }
            // 1 corner
            else if ((sides & 80) == 80) // neighboured on bottom and left
            {
                ReplaceMapEntry(dest, 31);
            }
            else if ((sides & 65) == 65) // neighboured on top and left
            {
                ReplaceMapEntry(dest, 32);
            }
            else if ((sides & 5) == 5) // neighboured on right and top
            {
                ReplaceMapEntry(dest, 33);
            }
            else if ((sides & 20) == 20) // neighboured on bottom and right
            {
                ReplaceMapEntry(dest, 34);
            }
            // opposite sides bordered
            else if ((sides & 17) == 17) // neighboured on top and bottom
            {
                ReplaceMapEntry(dest, 10);
            }
            else if ((sides & 68) == 68) // neighboured on left and right
            {
                ReplaceMapEntry(dest, 13);
            }

            //neighboured on 1 side
            else if ((sides & 16) == 16) // neighboured on bottom
            {
                ReplaceMapEntry(dest, 9);
            }
            else if ((sides & 1) == 1) // neighboured on top
            {
                ReplaceMapEntry(dest, 11);
            }
            else if ((sides & 4) == 4) // neighboured on right
            {
                ReplaceMapEntry(dest, 12);
            }
            else if ((sides & 64) == 64) // neighboured on left
            {
                ReplaceMapEntry(dest, 14);
            }
        }
    }

    private void AddMapEntry(Vector3Int dest, int index, int state)
    {
        GameObject obj = Instantiate(tiles[index].states[state].model, dest, Quaternion.identity, transform);
        tileMap.Add(dest, (index, obj));
    }

    private void ReplaceMapEntry(Vector3Int dest, int state)
    {
        GameObject oObj = tileMap[dest].Item2;
        int index = tileMap[dest].Item1;
        Destroy(oObj); 
        GameObject nObj = Instantiate(tiles[index].states[state].model, dest, Quaternion.identity, transform);
        tileMap[dest] = (index, nObj);
    }

    private void NullMapEntry(Vector3Int dest)
    {
        GameObject oObj = tileMap[dest].Item2;
        int index = tileMap[dest].Item1;
        Destroy(oObj);
        tileMap[dest] = (index, null);
    }

    private void Remove()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            Vector3 pos = hit.collider.transform.position;
            Vector3Int dest = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
            GameObject obj = tileMap[dest].Item2;
            tileMap.Remove(dest);
            Destroy(obj); 

            if (tileMap.ContainsKey(dest + new Vector3Int(0, -1, 0)))
            {
                UpdateTile(dest + new Vector3Int(0, -1, 0));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(0, 0, 1)))
            {
                UpdateTile(dest + new Vector3Int(0, 0, 1));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(1, 0, 1)))
            {
                UpdateTile(dest + new Vector3Int(1, 0, 1));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(1, 0, 0)))
            {
                UpdateTile(dest + new Vector3Int(1, 0, 0));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(1, 0, -1)))
            {
                UpdateTile(dest + new Vector3Int(1, 0, -1));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(0, 0, -1)))
            {
                UpdateTile(dest + new Vector3Int(0, 0, -1));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(-1, 0, -1)))
            {
                UpdateTile(dest + new Vector3Int(-1, 0, -1));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 0)))
            {
                UpdateTile(dest + new Vector3Int(-1, 0, 0));
            }
            if (tileMap.ContainsKey(dest + new Vector3Int(-1, 0, 1)))
            {
                UpdateTile(dest + new Vector3Int(-1, 0, 1));
            }
        }
    }
}
