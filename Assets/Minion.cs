using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private float speed = 2.0f;
    public bool isMoving = false;
    public Vector2 endPosition;
    MapGenerator mapGenerator;
    Rigidbody2D m_Rigidbody;
    Pathfinding2D pathfinder;

    public enum Intent {
        Idle,
        Attacking,
        Building,
        Moving,
    }

    public MapGenerator.Owner owner = MapGenerator.Owner.Neutral;

    public Intent intent;

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        pathfinder = FindObjectOfType<MinionParent>().GetComponent<Pathfinding2D>();
        Debug.Log("minion calling FindPath");
        pathfinder.FindPath(transform.position, endPosition);
        Debug.Log(pathfinder.path.ToArray());
    }

    void FixedUpdate()
    {
        if (isMoving) {
            var step =  speed * Time.deltaTime; // calculate distance to move
            // transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
            Vector2 direction = (endPosition - (Vector2) transform.position).normalized;
            m_Rigidbody.MovePosition((Vector2) transform.position + direction * step);
            // Debug.DrawLine(transform.position, endPosition, Color.red, 2.5f, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, endPosition) < 0.6f)
        {
            transform.position = endPosition;
            isMoving = false;
            Vector2Int gridPosition = mapGenerator.pixelToGrid(endPosition.x, endPosition.y);
            Debug.Log("grid position from minion: " + gridPosition);
            Debug.Log(mapGenerator);
            MapTile targetTile = mapGenerator.mapTileList[gridPosition.x][gridPosition.y];
            Debug.Log("grid position from target tile: " + targetTile.gridPosition);
            if (intent == Intent.Attacking) {
                // assert(targetTile.type == MapTile.TileType.Land);
                if (targetTile.owner != owner) {
                    targetTile.decrementStrength(owner); ;
                }

            } else if (intent == Intent.Building) {
                bool result = targetTile.buildBridge(owner);
                if (result) {
                    Vector2 newTileOffsets = mapGenerator.gridToPixel(gridPosition.x, gridPosition.y);
                    MapTile newTile = Instantiate(mapGenerator.bridgeTile,
                        new Vector3(newTileOffsets.x, newTileOffsets.y, 0), Quaternion.identity);
                    mapGenerator.mapTileList[gridPosition.x][gridPosition.y] = newTile;
                }
            }
            intent = Intent.Idle;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("GameObject1 collided with " + col.name);
    }
}
