using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapTile defaultTile;

    public List<List<MapTile>> mapTiles;
    public int sizeX = 16;
    public int sizeY = 16;

    public GameObject landSprite;
    public GameObject waterSprite;
    public GameObject tileSprite;

    HashSet<int> arrayIndices;

    public void GenerateLandTiles() {
        int lowerBound = (sizeX * sizeY) / 5;
        int upperBound = (sizeX * sizeY) / 4;
        int landTileCount = UnityEngine.Random.Range(lowerBound, upperBound + 1);
        arrayIndices = new HashSet<int>();
        // generate landTileCount number of ints, use them as indices into the 2d array
        for (int i = 0; i < landTileCount; ++i) {
            int newIndex = UnityEngine.Random.Range(0, sizeX * sizeY);
            while (arrayIndices.Contains(newIndex)) {
                newIndex = UnityEngine.Random.Range(0, sizeX * sizeY);
            }
            arrayIndices.Add(newIndex);
        }
    }
    
    bool isLandTile(int i, int j) {
        return arrayIndices.Contains((sizeX * i) + j);
        //return i > 4 && j > 4;
    }

    public void GenerateMap2() {
        // generate the tiles from right to left due to overlapping
        for (int i = sizeX - 1; i >= 0; --i) {
            for (int j = 0; j < sizeY; ++j) {
                float xOffset = (i + j) / 2f - sizeX/2;
                float yOffset = (i - j) / 4f;
                if (isLandTile(i, j)) {
                    GameObject tile = Instantiate(landSprite, new Vector3(xOffset, yOffset, 0), Quaternion.identity);
                } else {
                    GameObject tile = Instantiate(waterSprite, new Vector3(xOffset, yOffset, 0), Quaternion.identity);
                }
            }
        }
    }

    public void GenerateMap() {

        for (int i = 0; i < sizeX; ++i) {
            mapTiles.Add(new List<MapTile>());
            for (int j = 0; j < sizeY; ++j) {
                //MapTile newTile = Instantiate(defaultTile, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                //newTile.transform.SetParent(transform);
                /*
                Vector3 position = new Vector3(
                    //i * tileWidth + (j % 2) * (tileWidth / 2f),
                    i * tileWidth,
                    0,
                    j * tileHeight
                );
                */
                //newTile.transform.position = position;
                /*
                newTile.type = MapTile.TileType.Land;
                newTile.spriteRenderer.sprite = newTile.landSprite;
                mapTiles[i].Add(newTile);
                */ 
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        //mapTiles = new List<List<MapTile>>();
        GenerateLandTiles();
        GenerateMap2();
    }

    // Update is called once per frame
    void Update() {

    }
}
