using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Collections.Specialized;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public MapTile defaultTile;
    public MapTile playerTile;
    public MapTile opponentTile;
    public MapTile bridgeTile;

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

        if (visited[i, j] || !isLandTile(i, j))
            return;

        visited[i, j] = true;

        island.Add((i, j));

        DFS(i + 1, j, island);
        DFS(i - 1, j, island);
        DFS(i, j + 1, island);
        DFS(i, j - 1, island);
    }


    public void GenerateMap() {
        // generate the tiles from right to left due to overlapping
        float ratio = 1.5f;
        float scale = 0.6f;
        for (int i = sizeX - 1; i >= 0; --i) {
            for (int j = 0; j < sizeY; ++j) {
                float xOffset = (i + j) * scale;
                float yOffset = ((j - i) / ratio) * scale;
                if (isLandTile(i, j)) {
                    mapTileList[i].Add(Instantiate(defaultTile, new Vector3(xOffset, yOffset, 0), Quaternion.identity));
                }
            }
        }
        // add the player and opponent tiles at the end
        int playerI = playerIndex / sizeX;
        int playerJ = playerIndex % sizeX;
        float playerXOffset = (playerI + playerJ) * scale;
        float playerYOffset = (playerJ - playerI) / ratio * scale;
        Instantiate(playerTile, new Vector3(playerXOffset, playerYOffset, 0), Quaternion.identity);

        int opponentI = opponentIndex / sizeX;
        int opponentJ = opponentIndex % sizeX;
        float opponentXOffset = (opponentI + opponentJ) * scale;
        float opponentYOffset = (opponentJ - opponentI) / ratio * scale;
        Instantiate(opponentTile, new Vector3(opponentXOffset, opponentYOffset, 0), Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start() {
        GenerateLandTiles();
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
