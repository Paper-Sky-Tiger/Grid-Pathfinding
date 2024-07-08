using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Controller : MonoBehaviour
{
    private Grid grid;
    public Pathfinding pathfinding_script;
    public int gridSize = 10;
    public float cellSize = 6;
    public Vector2 StartingTilePos;
    public Vector2 FinishTilePos;
    
    void Start()
    {
        grid = new Grid(gridSize, gridSize, cellSize);
        pathfinding_script.grid = grid;

        StartingTilePos = new Vector2(0,0);
        FinishTilePos = new Vector2(gridSize-1,gridSize-1);
    }

    float updateFrequency = 0.1f;
    float currentElapsedTime = 0f;
    void FixedUpdate()
    {   
        currentElapsedTime += 0.02f;
        if(currentElapsedTime >= updateFrequency)
           currentElapsedTime = 0f;
        else return;

        UpdateStartFinishIndexes();

        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grid.SetTileValue(mousePos, grid.GetTileValue(mousePos)+1);
        }

        if(Input.GetMouseButton(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grid.SetTileValue(mousePos, grid.GetTileValue(mousePos)-1);
        }    
    }

    void Update()
    {
        grid.DrawGrid();
    }

    [Dropdown("Algorithm")]
    public string algorithm;
    private List<string> Algorithm { get { return new List<string>() { "A*", "Djikstra"};}}


    [Button("Find Path")]
    private void FindPath()
    {
        pathfinding_script.FindPath(grid.startTile, grid.finishTile, algorithm);
        pathfinding_script.DrawFoundPath(1);
    }

    [Button("Clean Path")]
    private void CleanPath()
    {
        grid.CleanTiles();
    }

    [Button("Reset Grid")]
    private void ResetGrid()
    {
        grid.ResetGridTiles();
    }

    [Button("Resize Grid")] // Specify button text
    private void ResizeGrid()
    {
        grid.destroyTileTextArray();
        grid = new Grid(gridSize,gridSize, cellSize);
        pathfinding_script.grid = grid;
        StartingTilePos = new Vector2(0,0);
        FinishTilePos = new Vector2(gridSize-1,gridSize-1);
        AdjustCamera();
    }

    private void UpdateStartFinishIndexes()
    {
        int maxX = gridSize-1;
        int maxY = gridSize-1;
        StartingTilePos.x = Mathf.Clamp(StartingTilePos.x, 0, maxX);
        StartingTilePos.y = Mathf.Clamp(StartingTilePos.y, 0, maxY);

        FinishTilePos.x = Mathf.Clamp(FinishTilePos.x, 0, maxX);
        FinishTilePos.y = Mathf.Clamp(FinishTilePos.y, 0, maxY);
        
        grid.SetStartTile(StartingTilePos);
        grid.SetFinishTile(FinishTilePos);
    }

    private void AdjustCamera()
    {
       Vector3 center = grid.GetGridCenter();
       center.z = -10;    
       Camera.main.transform.position = center; 
       Camera.main.orthographicSize = grid.GetGridSize() / 2 + 1;
    }
}
