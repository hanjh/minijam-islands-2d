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

    public enum TileOwner
    {
        Player,
        Opponent,
        Neutral,
    }

    public TileOwner owner = TileOwner.Neutral;

    public int strength = 10;

    public TileType type;

    public SpriteRenderer spriteRenderer;
    public Sprite tileSprite;

    public void incrementStrength(int minionCount)
    {
        strength += minionCount;
    }

    public void decrementStrength(int minionCount)
    {
        if (minionCount > strength)
        {
            strength = 0;
            owner = TileOwner.Opponent; // invert for opponent->owner, but this should be based on minion
            // once you take over a tile, what should the default strength be?
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
