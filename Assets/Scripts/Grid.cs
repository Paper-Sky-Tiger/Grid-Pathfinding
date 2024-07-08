using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private float cellSize;

    private Tile [,] gridArray {get;}
    private TextMesh[,] tileTextArray;
    
    public Tile startTile;
    public Tile finishTile;

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new Tile[width, height];
        tileTextArray = new TextMesh[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridArray[x,y] = new Tile(x,y,0);
                tileTextArray[x,y] = CreateTileText("0", GetWorldPosition(x,y) + new Vector3(cellSize, cellSize) * .5f, Color.white, 24);
            }
        }
    }

    public void DrawGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y + 1), Color.white);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x + 1, y), Color.white);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width,height), Color.white);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width,height), Color.white);
    }

    public void DrawPath(List<Tile> path)
    {
        foreach (Tile tile in path)
        {
            tileTextArray[tile.x,tile.y].text = "▇";
            tileTextArray[tile.x,tile.y].color = Color.green;
        }
    }

    public void SetTileValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetTileXY(worldPosition, out x, out y);
        SetTileValue(x, y, value);
    }

    public int GetTileValue(Vector3 worldPosition)
    {
        int x,y;
        GetTileXY(worldPosition, out x, out y);
        return GetTileValue(x,y);
    }

    public void SetStartTile(Vector2 index)
    {
        if(startTile != null)
            ResetTileText(startTile);
        int x = (int)index.x;
        int y = (int)index.y;

        startTile = gridArray[x,y];
        tileTextArray[x,y].text = "S";
        tileTextArray[x,y].color = Color.yellow;
    }

    public void SetFinishTile(Vector2 index)
    {
        if(finishTile != null)
            ResetTileText(finishTile);
        int x = (int)index.x;
        int y = (int)index.y;
        
        finishTile = gridArray[x,y];
        tileTextArray[x,y].text = "E";
        tileTextArray[x,y].color = Color.green;
    }

    public Vector3 GetGridCenter()
    {
        return new Vector3((float)width / 2 * cellSize, (float)height / 2 * cellSize, 0);
    }

    public int GetGridSize()
    {
        return width * (int)cellSize;
    }

    public void CreateTileConnections()
    {
        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {   
                gridArray[x,y].neighbours = new Tile[4];
                gridArray[x,y].minCostFromStart = -1;
                if(isGridIndexValid(x, y + 1))
                    gridArray[x,y].neighbours[0] = gridArray[x, y + 1];

                if(isGridIndexValid(x, y - 1))
                    gridArray[x,y].neighbours[1] = gridArray[x, y - 1];

                if(isGridIndexValid(x + 1, y))
                    gridArray[x,y].neighbours[2] = gridArray[x + 1, y];

                if(isGridIndexValid(x - 1, y))
                    gridArray[x,y].neighbours[3] = gridArray[x - 1, y];
            }
        }
    }

    public bool isGridIndexValid(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < width && y < height && gridArray[x,y].isValid);
    }

    public void CleanTiles()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridArray[x,y].isVisited = false;
                if(!(tileTextArray[x,y].text == "▇" && tileTextArray[x,y].color == Color.red))
                {
                    tileTextArray[x,y].text = gridArray[x,y].value.ToString();
                    tileTextArray[x,y].color = Color.white;
                }
            }
        }
    }

    public void SetTileVisited(Tile tile, bool changeText = true)
    {
        tile.isVisited = true;
        if(changeText)
        {
            tileTextArray[tile.x,tile.y].text = "▇";
            tileTextArray[tile.x,tile.y].color = Color.black;
        }
    }

    public void SetPathTile(Tile tile)
    {
        tileTextArray[tile.x,tile.y].text = "▇";
        tileTextArray[tile.x,tile.y].color = Color.green;
    }

    public void destroyTileTextArray()
    {
        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
                Destroy(tileTextArray[x,y].gameObject);
            }
        }
    }

    public void ResetGridTiles()
    {
        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x,y] = new Tile(x,y,0);
                gridArray[x,y].neighbours = new Tile[4];
                Destroy(tileTextArray[x,y].gameObject);
                tileTextArray[x,y] = CreateTileText(gridArray[x,y].value.ToString(), GetWorldPosition(x,y) + new Vector3(cellSize, cellSize) * .5f, Color.white, 24);
            }
        }
    }

    public void SetTileDisplayValue(Tile tile, int value)
    {
        tileTextArray[tile.x, tile.y].text = value.ToString();
        tileTextArray[tile.x, tile.y].color = Color.gray;
    }

    private void ResetTileText(Tile tile)
    {
        gridArray[tile.x,tile.y].value = 0;
        tileTextArray[tile.x,tile.y].text = gridArray[tile.x,tile.y].value.ToString();
        tileTextArray[tile.x,tile.y].color = Color.white;
    }

    private int GetTileValue(int x, int y)
    {
        if(x >= 0 && y>= 0 && x < width && y < height)
            return gridArray[x,y].value;
        else return 0;
    }

    private void SetTileValue(int x, int y, int value)
    {
        if(x >= 0 && y>= 0 && x < width && y < height)
        {
            value = Mathf.Clamp(value, -1, 10);
            gridArray[x,y].value = value;

            if(value == -1)
            {
                tileTextArray[x,y].text = "▇";
                tileTextArray[x,y].color = Color.red;
                gridArray[x,y].isValid = false;
            }
            else
            {
                tileTextArray[x,y].text = value.ToString();
                tileTextArray[x,y].color = Color.white;
                gridArray[x,y].isValid = true;
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y) * cellSize;
    }

    private void GetTileXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition).y / cellSize);
    }
    
    private TextMesh CreateTileText(string text, Vector3 localPosition, Color color, int fontSize = 40)
    {
       GameObject gameObject = new GameObject("Tile", typeof(TextMesh));
       gameObject.transform.localPosition = localPosition;
       TextMesh textMesh = gameObject.GetComponent<TextMesh>();
       textMesh.anchor = TextAnchor.MiddleCenter;
       textMesh.text = text;
       textMesh.fontSize = fontSize;
       textMesh.color = color;
       return textMesh;
    }

}
