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

    public void handleMouseDown() {
        Debug.Log("mouse button down, position: " + Input.mousePosition);
        int x = Input.mousePosition.x;
        int y = Input.mousePosition.y;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Debug.Log(hit.collider.gameObject.name);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            dragging = hit.transform;
            offset = dragging.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void handleMouseUp() {
        Debug.Log("mouse button up, position: " + Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Land"))
        {
            // find out which tile is at this position
            MoveToTarget(dragging.position);
            HandleAttack();
        }
        else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            MoveToTarget(dragging.position);
            HandleBuild();
        }
        dragging = null;
    }

    public void HandleAttack() {
        Debug.Log("attacking");
    }

    public void HandleBuild() {
        Debug.Log("Build!");
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void MoveToTarget(Vector3 startPosition) {
        Debug.Log("Moving to target");
        GameObject newMinion = Instantiate(playerMinion, new Vector3(startPosition.x, startPosition.y, 0), Quaternion.identity);
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newMinion.GetComponent<Minion>().endPosition = endPosition;
        newMinion.GetComponent<Minion>().isMoving = true;
    }


}
