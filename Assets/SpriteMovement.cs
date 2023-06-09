using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMovement : MonoBehaviour
{
    public float moveSpeed = 20f; // Speed at which the character moves

    private Rigidbody2D rb;

    private float moveH;
    private float moveV;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get input from arrow keys
        moveH = Input.GetAxisRaw("Horizontal");
        moveV = Input.GetAxisRaw("Vertical");

        // Calculate movement vector
        Vector2 movement = new Vector2(moveH, moveV) * moveSpeed;

        // Move the character
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveH, moveV);
    }

}
