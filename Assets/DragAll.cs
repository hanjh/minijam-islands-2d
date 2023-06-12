using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAll : MonoBehaviour
{
    public Transform dragging = null;
    public Vector3 offset;
    [SerializeField] private LayerMask movableLayers;
    public GameObject playerMinion;
    public GameObject enemyMinion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log("checking if can move....");
            // Debug.Log(movableLayers);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) {
                Debug.Log("can move!");
                dragging = hit.transform;
                offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        } else if (Input.GetMouseButtonUp(0) && dragging != null) {
            Debug.Log("mouse button up");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Land")) {
                handleAttack(dragging.position);
            } else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water")) {
                handleBuild();
            }
            dragging = null;
        }

        // if (dragging != null) {
        //     dragging.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        // }
    }

    public void handleAttack(Vector3 startPosition) {
        Debug.Log("Attack!");
        GameObject newMinion = Instantiate(playerMinion, new Vector3(startPosition.x, startPosition.y, 0), Quaternion.identity);
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newMinion.GetComponent<MinionPathing>().endPosition = endPosition;
        newMinion.GetComponent<MinionPathing>().isMoving = true;
    }

    public void handleBuild() {
        Debug.Log("Build!");
    }
}
