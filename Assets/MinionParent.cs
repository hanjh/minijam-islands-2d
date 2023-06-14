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

    public GameObject mouseArrow;
    public GameObject activeArrow;
    public bool isActiveArrow = false;
    public Vector2 tailPosition;
    public float defaultArrowWidth = 0;
    public float defaultArrowLength = 0;


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
       if (isActiveArrow == false) {
            Debug.Log("Instantiating new arrow");
            Vector2 centeredCoordinates = mapGenerator.gridToPixel(gridPosition.x, gridPosition.y);
            activeArrow = Instantiate(mouseArrow, new Vector3(centeredCoordinates.x, centeredCoordinates.y, 0), 
                Quaternion.identity);
            isActiveArrow = true;
            Bounds arrowBounds = activeArrow.GetComponent<SpriteRenderer>().bounds;
            defaultArrowWidth = arrowBounds.size.x;
            defaultArrowLength = arrowBounds.size.y;
            Debug.Log("defaultArrowWidth :" + defaultArrowWidth + " defaultArrowLength: " + defaultArrowLength);
            tailPosition = new Vector2(centeredCoordinates.x, centeredCoordinates.y);
            //Vector2 offset = new Vector2(activeArrow.transform.position.x, 
            //    activeArrow.transform.position.y + defaultArrowLength/2);
            //activeArrow.transform.position = tailPosition + offset;
        } else {
        }
    }

    public void handleMouseUp() {
        Debug.Log("mouse button up, position: " + Input.mousePosition);
        Vector2Int gridPosition = ScreenPositionToGridPosition(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Land"))
        {
            GameObject newMinion = Instantiate(playerMinion, new Vector3(dragging.position.x, dragging.position.y, 0), 
                Quaternion.identity);
            newMinion.GetComponent<Minion>().intent = Minion.Intent.Attacking;
            newMinion.GetComponent<Minion>().owner = MapGenerator.Owner.Player;
            MoveToTarget(newMinion);
        }
        else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GameObject newMinion = Instantiate(playerMinion, new Vector3(dragging.position.x, dragging.position.y, 0), 
                Quaternion.identity);
            newMinion.GetComponent<Minion>().intent = Minion.Intent.Building;
            newMinion.GetComponent<Minion>().owner = MapGenerator.Owner.Player; 
            MoveToTarget(newMinion);
        }
        dragging = null;
        if (isActiveArrow == true) {
            isActiveArrow = false;
            Destroy(activeArrow);
        }
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
        if (isActiveArrow == true) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 headPosition = new Vector2(mousePosition.x, mousePosition.y);
            activeArrow.transform.position = (headPosition - tailPosition) / 2 + tailPosition;
            Vector2 direction = headPosition - tailPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            activeArrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            float distance = Vector2.Distance(tailPosition, headPosition);
            float newScale = distance / defaultArrowLength;
            Debug.Log("distance: " + distance + " defaultArrowLength: " + defaultArrowLength + " newScale: " + newScale);
            activeArrow.transform.localScale = new Vector3(0.2f, newScale, 0.2f);
        }
    }


}
