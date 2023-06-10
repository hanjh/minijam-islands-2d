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
        int lowerBound = (sizeX * sizeY) / 4;
        int upperBound = (sizeX * sizeY) / 3;
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
    }

    public void GenerateMap() {
        // generate the tiles from right to left due to overlapping
        float ratio = 1.5f;
        float scale = 0.6f;
        for (int i = sizeX - 1; i >= 0; --i) {
            for (int j = 0; j < sizeY; ++j) {
                float xOffset = (i + j) * scale;
                float yOffset = ((i - j) / ratio) * scale;
                if (isLandTile(i, j)) {
                    GameObject tile = Instantiate(landSprite, new Vector3(xOffset, yOffset, 0), Quaternion.identity);
                }
            }
        }
    }

  // Start is called before the first frame update
    void Start() {
        GenerateLandTiles();
        GenerateMap();
    }

    // Update is called once per frame
    void Update() {

    }
}
