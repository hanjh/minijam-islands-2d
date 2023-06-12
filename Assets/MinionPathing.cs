using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionPathing : MonoBehaviour
{
    private float speed = 2.0f;
    public bool isMoving = false;
    public Vector2 endPosition;
    Rigidbody2D m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        // Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.white, 2.5f);
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
        if (Vector3.Distance(transform.position, endPosition) < 0.05f)
        {
            transform.position = endPosition;
            isMoving = false;
        }
    }
}
