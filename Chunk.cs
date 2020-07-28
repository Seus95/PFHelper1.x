using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static readonly int chunkSize = 10;
    public Vector2 coord;
    public GameObject chunkObj;

    public TileSlot[,] map;

    public Chunk(int x, int y)
    {
        coord = new Vector2(x, y);
        chunkObj = new GameObject("chunk " + x + " " + y);
        chunkObj.transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
        map = new TileSlot[chunkSize, chunkSize];
    }

}
