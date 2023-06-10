using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public enum TileType
    {
        Land,
        Water,
    }
    public TileType type;

    public SpriteRenderer spriteRenderer;
    public Sprite tileSprite;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
