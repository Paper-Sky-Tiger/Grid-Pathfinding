using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Grid grid {get; set;}
    List<Tile> visitedTiles = new List<Tile>();
    List<Tile> pathFound = new List<Tile>();
    public float delay;

    IEnumerator paintVisitedTiles()
    {
        PrintResults();
        foreach (Tile tile in visitedTiles)
        {
            grid.SetTileVisited(tile);
            yield return new WaitForSeconds(delay);
        }
        visitedTiles.Clear();
    }

    IEnumerator paintVisitedTileCosts()
    {
        PrintResults();
        foreach (Tile tile in visitedTiles)
        {
            grid.SetTileDisplayValue(tile, tile.minCostFromStart);
            yield return new WaitForSeconds(delay/10);
        }
        visitedTiles.Clear();
    }

    IEnumerator drawFoundPath(float startAfter)
    {
        yield return new WaitForSeconds(startAfter);
        pathFound.Reverse();
        foreach (Tile tile in pathFound)
        {
            grid.SetPathTile(tile);
            yield return new WaitForSeconds(delay);
        }
        pathFound.Clear();
    }

    public void DrawFoundPath(float startAfter)
    {
        StartCoroutine(drawFoundPath(startAfter));
    }

    private void PrintResults()
    {
        Debug.Log("Visited tiles count: " + visitedTiles.Count);
        Debug.Log("Path tiles count: " + pathFound.Count);

        int total = 0;
        foreach(Tile tile in pathFound)
            total += tile.value;

        Debug.Log("Found path cost: " + total);
    }

    void Start()
    {
        delay = 0.1f;
    }

    public bool FindPath(Tile startTile, Tile finishTile, string algorithm)
    {
        grid.CreateTileConnections();
        grid.CleanTiles();

        switch(algorithm)
        {
            case "A*" : 
                return a_star(startTile, finishTile);
                break;
            case "Djikstra" :
                return djikstra(startTile,finishTile);
                break;
            default:
                return a_star(startTile, finishTile);
        }
    }

    private bool a_star(Tile startTile, Tile finishTile)
    {
        List<Tile> openList = new List<Tile>();
        openList.Add(startTile);
        Tile currentBestTile = new Tile(-1,-1,-1);

        int count = 0;
        while(openList.Count != 0)
        {   
            count++;
            currentBestTile = RemoveBestTile(openList);
            
            if(currentBestTile == finishTile)
            {
                Tile tile = finishTile;
                while(tile != null)
                {
                    pathFound.Add(tile);
                    tile = tile.parent;
                }
                StartCoroutine(paintVisitedTiles());
                return true;
            }
            else
            {
                for(int i = 0; i < 4; ++i)
                {
                    Tile neighbour = currentBestTile.neighbours[i];
                    
                    if(neighbour == null)
                        continue;

                    if(neighbour.isVisited)
                        continue;

                    float f = Heuristic(neighbour,finishTile);
                    
                    bool inOpen = openList.Contains(neighbour);

                    if(!inOpen)
                        openList.Add(neighbour);
                    
                    if(!inOpen || f < neighbour.f)
                    {
                        neighbour.parent = currentBestTile;
                        neighbour.f = f;
                    }
                }
                
                grid.SetTileVisited(currentBestTile, false);
                visitedTiles.Add(currentBestTile);
            }
        }
        StartCoroutine(paintVisitedTiles());
        return false;
    } 

    private bool djikstra(Tile startTile, Tile finishTile)
    {
        bool found = false;
        startTile.minCostFromStart = 0;
        List<Tile> openList = new List<Tile>();
        openList.Add(startTile);

        while(openList.Count != 0)
        {
            Tile currentTile = openList[0];
            openList.Remove(currentTile);
            currentTile.isVisited = true;

            foreach (Tile neighbour in currentTile.neighbours)
            {   
                if(neighbour == null)
                    continue;

                if(neighbour.isVisited)
                    continue;

                if(neighbour == finishTile)
                    found = true;
                
                if(!openList.Contains(neighbour))
                    openList.Add(neighbour);

                if(neighbour.minCostFromStart == -1 || currentTile.minCostFromStart + neighbour.value < neighbour.minCostFromStart)
                {
                    neighbour.minCostFromStart = currentTile.minCostFromStart + neighbour.value;
                    neighbour.parent = currentTile;
                }

                visitedTiles.Add(neighbour);
            }
        }

        if(found)
        {
            Tile tile = finishTile.parent;
            while(tile != startTile)
            {
                pathFound.Add(tile);
                tile = tile.parent;
            }
        }

        StartCoroutine(paintVisitedTileCosts());
        return found;
    }

    private Tile RemoveBestTile(List<Tile> list)
    {
        Tile best = list[0];

        foreach (Tile tile in list)
        {
            if(tile.f + tile.value < best.f + best.value)
                best = tile;
        }

        list.Remove(best);

        return best;
    }

    private float Heuristic(Tile from, Tile to)
    {
        return new Vector2(from.x - to.x, from.y - to.y).magnitude;
    }
}
