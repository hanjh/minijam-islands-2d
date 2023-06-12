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

    public enum Owner
    {
        Player,
        Opponent,
        Neutral,
    }

    public TileOwner = Neutral;

    public int strength = 10;

    public TileType type;

    public SpriteRenderer spriteRenderer;
    public Sprite tileSprite;

    public void incrementStrength(MinionGroup group)
    {
        strength += group.size;
    }

    public void decrementStrength(MinionGroup group)
    {
        if (group.size > strength)
        {
            group.size = group.size - strength;
            strength = 0;
            TileOwner = group.owner;
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
