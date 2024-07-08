using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x {get;}
    public int y {get;}

    public Tile[] neighbours {get; set;}
    public Tile parent {get; set;}
    public int value {get; set;}
    public float f {get; set;}
    public int minCostFromStart {get; set;}
    public bool isVisited {get; set;}
    public bool isValid {get; set;}

    public Tile(int x, int y, int value = 0)
    {
        this.x = x;
        this.y = y;
        this.f = 0;
        this.value = value;
        this.isValid = true;
        this.minCostFromStart = -1;
        this.parent = null;
        this.isVisited = false;
        this.neighbours = new Tile[4];
    }
}
