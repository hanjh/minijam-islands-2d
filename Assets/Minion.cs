using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    private float speed = 2.0f;
    public bool isMoving = false;
    public Vector3 endPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            var step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, endPosition, step);
        }

        if (Vector3.Distance(transform.position, endPosition) < 0.001f)
        {
            transform.position = endPosition;
            isMoving = false;
        }
    }
}
