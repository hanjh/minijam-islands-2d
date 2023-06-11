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

    public GameObject colliderParent;
    public Dictionary<Tuple<int, int>, GameObject> colliderMap = new Dictionary<Tuple<int, int>, GameObject>();

    public List<PolygonCollider2D> collidersToVisualize = new List<PolygonCollider2D>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    private void OnDrawGizmos()
    {
        Vector3 cubePosition = new Vector3(8f, -2f, 0f);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(cubePosition, Vector3.one);
    }

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

        if (visited[i, j])
        {
            return;
        }
        if (!isLandTile(i, j))
        {
            // GameObject colliderObject = new GameObject();
            // colliderObject.transform.parent = colliderParent.transform;

            // PolygonCollider2D collider = colliderObject.AddComponent<PolygonCollider2D>();

            // // Tuple<float, float> offsets = gridToPixel(i, j);
            // // colliderObject.transform.position = new Vector3(offsets.Item1, offsets.Item2, 0);
            // // collider.size = defaultTileSize;

            // Tuple<int, int> position = Tuple.Create(i, j);
            // if (!colliderMap.ContainsKey(position))
            // {
            //     colliderMap.Add(position, colliderObject);
            // }
            // collidersToVisualize.Add(collider);

            return;
        }

        visited[i, j] = true;

        island.Add((i, j));

        DFS(i + 1, j, island);
        DFS(i - 1, j, island);
        DFS(i, j + 1, island);
        DFS(i, j - 1, island);
    }

    Tuple<float, float> gridToPixel(int i, int j)
    {
        float ratio = 1.5f;
        float scale = 0.6f;
        return Tuple.Create((i + j) * scale, ((j - i) / ratio) * scale);
    }

    public void GenerateMap() {
        // generate the tiles from right to left due to overlapping
        for (int i = 0; i < sizeX; ++i) {
            for (int j = sizeY - 1; j >= 0; --j) {
                Tuple<float, float> tileOffsets = gridToPixel(i, j);
                if (isLandTile(i, j)) {
                    MapTile newTile = Instantiate(defaultTile, 
                        new Vector3(tileOffsets.Item1, tileOffsets.Item2, 0), Quaternion.identity);
                    mapTileList[i].Add(newTile);
                    // collidersToVisualize.Add(newTile.GetComponent<PolygonCollider2D>());
                } else {
                    Instantiate(waterTile, 
                        new Vector3(tileOffsets.Item1, tileOffsets.Item2, 0), Quaternion.identity);
                }
            }
        }
        // add the player and opponent tiles at the end
        int playerI = playerIndex / sizeX;
        int playerJ = playerIndex % sizeX;
        Tuple<float, float> offsets = gridToPixel(playerI, playerJ);
        Instantiate(playerTile, new Vector3(offsets.Item1, offsets.Item2, 0), Quaternion.identity);

        int opponentI = opponentIndex / sizeX;
        int opponentJ = opponentIndex % sizeX;
        offsets = gridToPixel(opponentI, opponentJ);
        Instantiate(opponentTile, new Vector3(offsets.Item1, offsets.Item2, 0), Quaternion.identity);
    }

    void CreateLineRenderers()
    {
        for (int i = 0; i < collidersToVisualize.Count; i++)
        {
            GameObject lineRendererObject = new GameObject("LineRenderer");
            lineRendererObject.transform.SetParent(transform);

            LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 5;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            lineRenderers.Add(lineRenderer);
        }
    }

// Start is called before the first frame update
void Start() {
        GenerateLandTiles();
        defaultTileSize = defaultTile.GetComponent<SpriteRenderer>().bounds.size;
        colliderParent = new GameObject();
        List<List<(int, int)>> islands = FindIslands();

        mapTileList = new List<List<MapTile>>();
        for (int i = 0; i < sizeX; ++i)
        {
            mapTileList.Add(new List<MapTile>());
        }
        GenerateMap();
        // CreateLineRenderers();
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < collidersToVisualize.Count; i++)
        {
            if (collidersToVisualize[i] != null)
            {
                Vector2 min = collidersToVisualize[i].bounds.min;
                Vector2 max = collidersToVisualize[i].bounds.max;

                lineRenderers[i].SetPosition(0, new Vector3(min.x, min.y, 0f));
                lineRenderers[i].SetPosition(1, new Vector3(max.x, min.y, 0f));
                lineRenderers[i].SetPosition(2, new Vector3(max.x, max.y, 0f));
                lineRenderers[i].SetPosition(3, new Vector3(min.x, max.y, 0f));
                lineRenderers[i].SetPosition(4, new Vector3(min.x, min.y, 0f));
            }
        }

    }
}
