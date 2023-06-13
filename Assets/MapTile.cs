using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;


public class MapTile : MonoBehaviour
{
    public enum TileType
    {
        Land,
        Water,
        Bridge,
    }

    public Vector2Int gridPosition = new Vector2Int(0, 0);

    public MapGenerator.Owner owner = MapGenerator.Owner.Neutral;

    public int strength = 2;

    public int bridgeCost = 1;
    // what if two people build a bridge at once?

    public TileType type;

    public SpriteRenderer spriteRenderer;
    public Sprite tileSprite;

    public bool readyToDestroy = false;

    public void incrementStrength()
    {
        strength++;
    }

    public void decrementStrength(MapGenerator.Owner attackingOwner)
    {
        strength--;
        if (strength == 0) {
            owner = attackingOwner;
            Debug.Log("changing owner for tile " + gridPosition);
            // apply appropriate color transforms
            if (owner == MapGenerator.Owner.Player) {
                spriteRenderer.color = Color.blue;
            } else {
                spriteRenderer.color = Color.red;
            }
        }
    }
    
    public bool buildBridge(MapGenerator.Owner buildingOwner) {
        bridgeCost--;
        if (bridgeCost == 0) {
            readyToDestroy = true;
            return true;
        }
        return false;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (readyToDestroy) {
            Destroy(gameObject);
        }
    }
}
