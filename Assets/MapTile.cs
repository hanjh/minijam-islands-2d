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
    public Sprite landSprite;
    public Sprite waterSprite;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MapTile position: " + transform.position);
        Debug.Log("maptile type: " + type);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
