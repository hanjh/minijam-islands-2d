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
    public static int sizeX = 16;
    public static int sizeY = 16;

    public int playerIndex;
    public int opponentIndex;

    public GameObject landSprite;
    public GameObject waterSprite;
    public GameObject tileSprite;

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

    Tuple<float, float> gridToPixel(int i, int j) {
        float ratio = 1.5f;
        float scale = 0.6f;
        return Tuple.Create((i + j) * scale, ((j - i) / ratio) * scale);
    }

    Tuple<int, int> pixelToGrid(float x, float y) {
        float ratio = 1.5f;
        float scale = 0.6f;

        int i = (int)Math.Round((x / scale) - (y / (ratio * scale)));
        int j = (int)Math.Round((x / scale) + (y / (ratio * scale)));

        return Tuple.Create(i, j);
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
        // generate the tiles from right to left due to overlapping
        for (int i = 0; i < sizeX; ++i) {
            for (int j = sizeY - 1; j >= 0; --j) {
                Tuple<float, float> tileOffsets = gridToPixel(i, j);
                MapTile currentTile = defaultTile;
                if (isPlayerTile(i, j)) {
                    currentTile = playerTile;
                } else if (isOpponentTile(i, j)) {
                    currentTile = opponentTile;
                } else if (isLandTile(i, j)) {
                    currentTile = defaultTile;
                } else {
                    currentTile = waterTile;
                }
                MapTile newTile = Instantiate(currentTile,
                        new Vector3(tileOffsets.Item1, tileOffsets.Item2, 0), Quaternion.identity);
                newTile.gridPosition.x = i;
                newTile.gridPosition.y = j;
                mapTileList[i].Add(newTile);
            }
        }
    }

// Start is called before the first frame update
void Start() {
        GenerateLandTiles();
        defaultTileSize = defaultTile.GetComponent<SpriteRenderer>().bounds.size;
        List<List<(int, int)>> islands = FindIslands();
        mapTileList = new List<List<MapTile>>();
        for (int i = 0; i < sizeX; ++i)
        {
            mapTileList.Add(new List<MapTile>());
        }
        GenerateMap();
    }

    // Update is called once per frame
    void Update() {

    }
}
