using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Collections.Specialized;
using System.Linq;
using System;

public class MapGenerator : MonoBehaviour
{
    public MapTile defaultTile;
    public MapTile waterTile;
    public MapTile playerTile;
    public MapTile opponentTile;
    public MapTile bridgeTile;

    public Sprite colliderVisual;

    public Vector2 defaultTileSize;

    public List<List<MapTile>> mapTileList;
    public Node2D[,] Grid;
    public static int sizeX = 16;
    public static int sizeY = 16;

    public int playerIndex;
    public int opponentIndex;

    public GameObject landSprite;
    public GameObject waterSprite;
    public GameObject tileSprite;

    // mapgenerator does not have an owner, this is a common type that is shared
    // between tiles and minions
    public enum Owner {
        Player,
        Opponent,
        Neutral,
    }

    HashSet<int> arrayIndices;
    private bool[,] visited = new bool[sizeX, sizeY];

    public void GenerateLandTiles() {
        int lowerBound = (sizeX * sizeY) / 4;
        int upperBound = (sizeX * sizeY) / 3;
        int landTileCount = UnityEngine.Random.Range(lowerBound, upperBound + 1);
        arrayIndices = new HashSet<int>();
        // generate landTileCount number of ints, use them as indices into the 2d array
        for (int i = 0; i < landTileCount; ++i) {
            int newIndex = UnityEngine.Random.Range(0, sizeX * sizeY);
            while (arrayIndices.Contains(newIndex)) { // in case of collisions
                newIndex = UnityEngine.Random.Range(0, sizeX * sizeY);
            }
            arrayIndices.Add(newIndex);
        }
        // Substitute two of the tiles for the player and opponent tiles
        playerIndex = arrayIndices.ElementAt(UnityEngine.Random.Range(0, arrayIndices.Count));
        arrayIndices.Remove(playerIndex);
        opponentIndex = arrayIndices.ElementAt(UnityEngine.Random.Range(0, arrayIndices.Count));
        arrayIndices.Remove(opponentIndex); 
    }
    
    bool isLandTile(int i, int j) {
        return arrayIndices.Contains((sizeX * i) + j);
    }

    public List<List<(int, int)>> FindIslands()
    {
        List<List<(int, int)>> islands = new List<List<(int, int)>>();
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                if (!visited[i, j] && isLandTile(i, j))
                {
                    List<(int, int)> island = new List<(int, int)>();
                    DFS(i, j, island);
                    islands.Add(island);
                }
            }
        }
        return islands;
    }

    private void DFS(int i, int j, List<(int, int)> island)
    {
        if (i < 0 || j < 0 || i >= sizeX || j >= sizeY)
            return;

        if (visited[i, j] || !isLandTile(i ,j))
        {
            return;
        }

        visited[i, j] = true;

        island.Add((i, j));

        DFS(i + 1, j, island);
        DFS(i - 1, j, island);
        DFS(i, j + 1, island);
        DFS(i, j - 1, island);
    }

    public Vector2 gridToPixel(int i, int j) {
        float ratio = 1.5f;
        float scale = 0.6f;
        return new Vector2((i + j) * scale, ((j - i) / ratio) * scale);
    }

    public Vector2Int pixelToGrid(float x, float y) {
        float ratio = 1.5f;
        float scale = 0.6f;

        float i = (x - y * ratio) / (2 * scale);
        float j = (y * ratio) / scale + (x - y * ratio) / (2 * scale);

        return new Vector2Int((int)Math.Round(i), (int)Math.Round(j));
    }

    public Node2D NodeFromPixel(Vector3 worldPosition)
    {
        Vector2Int gridPos = pixelToGrid(worldPosition.x, worldPosition.y);
        return Grid[gridPos.x, gridPos.y];
    }

    bool isPlayerTile(int i, int j) {
        int playerI = playerIndex / sizeX;
        int playerJ = playerIndex % sizeX;
        return i == playerI && j == playerJ;
    }

    bool isOpponentTile(int i, int j) {
        int opponentI = opponentIndex / sizeX;
        int opponentJ = opponentIndex % sizeX;
        return i == opponentI && j == opponentJ;
    }

    public void GenerateMap() {
        Grid = new Node2D[sizeX, sizeY];
        // generate the tiles from right to left due to overlapping
        for (int i = 0; i < sizeX; ++i) {
            for (int j = sizeY - 1; j >= 0; --j) {
                Vector2 tileOffsets = gridToPixel(i, j);
                MapTile currentTile = defaultTile;
                Grid[i, j] = new Node2D(false, gridToPixel(i, j), i, j);
                if (isPlayerTile(i, j)) {
                    currentTile = playerTile;
                } else if (isOpponentTile(i, j)) {
                    currentTile = opponentTile;
                } else if (isLandTile(i, j)) {
                    currentTile = defaultTile;
                } else {
                    currentTile = waterTile;
                    Grid[i, j].SetObstacle(true);
                }
                MapTile newTile = Instantiate(currentTile,
                        new Vector3(tileOffsets.x, tileOffsets.y, 0), Quaternion.identity);
                newTile.gridPosition.x = i;
                newTile.gridPosition.y = j;
                mapTileList[i][j] = newTile;
            }
        }
    }

    public List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.GridX >= 0 && node.GridX < sizeX && node.GridY + 1 >= 0 && node.GridY + 1 < sizeY)
            neighbors.Add(Grid[node.GridX, node.GridY + 1]);

        //checks and adds bottom neighbor
        if (node.GridX >= 0 && node.GridX < sizeX && node.GridY - 1 >= 0 && node.GridY - 1 < sizeY)
            neighbors.Add(Grid[node.GridX, node.GridY - 1]);

        //checks and adds right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1 < sizeX && node.GridY >= 0 && node.GridY < sizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY]);

        //checks and adds left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < sizeX && node.GridY >= 0 && node.GridY < sizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY]);

        return neighbors;
    }

    // Start is called before the first frame update
    void Start() {
        GenerateLandTiles();
        defaultTileSize = defaultTile.GetComponent<SpriteRenderer>().bounds.size;
        List<List<(int, int)>> islands = FindIslands();
        mapTileList = new List<List<MapTile>>();
        // prepopulate the list
        for (int i = 0; i < sizeX; ++i) {
            mapTileList.Add(new List<MapTile>());
            for (int j = 0; j < sizeY; ++j) {
                mapTileList[i].Add(null);
            }
        }
        GenerateMap();
    }

    // Update is called once per frame
    void Update() {

    }
}
