using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject obj;
    public readonly bool isBreakable;
    public readonly ToolType toolType;
    public int dropNumber = 1;
    public GameObject drobObj;

    private int maxTick;
    private int tick;

    public bool Break()
    {
        ++tick;
        return tick >= maxTick;
    }
    public int TickPercent { get { return tick * 100 / maxTick; } }
    public void ResetTick() { tick = 0; }
    public Tile(GameObject _obj, bool _breakable, int _maxTick, ToolType _tool = ToolType.HAND)
    {
        obj = _obj;
        isBreakable = _breakable;
        maxTick = _maxTick;
        tick = 0;
        toolType = _tool;
    }
}
public class TileSlot
{
    public GameObject back;
    public Tile front;
    public TileSlot(GameObject _back, Tile _front = null)
    {
        back = _back;
        front = _front;
    }
    public void SetTile(Tile tile)
    {
        if (front == null)
            front = tile;
    }
}
