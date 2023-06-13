using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionParent : MonoBehaviour
{
    public Transform dragging = null;
    public Vector3 offset;
    [SerializeField] private LayerMask movableLayers;
    public GameObject playerMinion;
    public GameObject enemyMinion;
    MapGenerator mapGenerator;

    MapGenerator.Owner owner;

    public Vector2Int ScreenPositionToGridPosition(Vector3 mousePosition) {
        Vector3 point = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2Int gridPosition = mapGenerator.pixelToGrid(point.x, point.y);
        Debug.Log("grid position: " + gridPosition);
        return gridPosition;
    }

    public void MoveToTarget(GameObject newMinion) {
        Debug.Log("Moving to target");
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        // Debug.DrawLine(endPosition, newMinion.transform.position, Color.blue, Mathf.Infinity, false);
        Vector2Int gridEndPosition = mapGenerator.pixelToGrid(endPosition.x, endPosition.y);
        Vector2 pixelEndPosition = mapGenerator.gridToPixel(gridEndPosition.x, gridEndPosition.y);
        // Debug.DrawLine(pixelEndPosition, newMinion.transform.position, Color.red, Mathf.Infinity, false);
        newMinion.GetComponent<Minion>().endPosition = pixelEndPosition;
        newMinion.GetComponent<Minion>().isMoving = true;
    }

    public void handleMouseDown() {
        Debug.Log("mouse button down, position: " + Input.mousePosition);
        Vector2Int gridPosition = ScreenPositionToGridPosition(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Debug.Log(hit.collider.gameObject.name);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) {
            dragging = hit.transform;
            offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void handleMouseUp() {
        Debug.Log("mouse button up, position: " + Input.mousePosition);
        Vector2Int gridPosition = ScreenPositionToGridPosition(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Land"))
        {
            GameObject newMinion = Instantiate(playerMinion, new Vector3(dragging.position.x, dragging.position.y, 0), Quaternion.identity);
            newMinion.GetComponent<Minion>().intent = Minion.Intent.Attacking;
            newMinion.GetComponent<Minion>().owner = MapGenerator.Owner.Player;
            MoveToTarget(newMinion);
            HandleAttack(gridPosition);
        }
        else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GameObject newMinion = Instantiate(playerMinion, new Vector3(dragging.position.x, dragging.position.y, 0), Quaternion.identity);
            newMinion.GetComponent<Minion>().intent = Minion.Intent.Building;
            newMinion.GetComponent<Minion>().owner = MapGenerator.Owner.Player; 
            MoveToTarget(newMinion);
            HandleBuild(gridPosition);
        }
        dragging = null;
    }

    public void HandleAttack(Vector2Int gridPosition) {
        MapTile tile = mapGenerator.mapTileList[gridPosition.x][gridPosition.y];
    }

    public void HandleBuild(Vector2 gridPosition) {
        Debug.Log("Build!");
    }

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>(); // ensure we only have one, or this breaks
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            handleMouseDown();
        } else if (Input.GetMouseButtonUp(0) && dragging != null) {
            handleMouseUp();
        }
    }


}
