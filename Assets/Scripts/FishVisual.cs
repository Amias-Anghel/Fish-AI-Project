using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishVisual : MonoBehaviour
{
    private bool facingLeft = true;

    [SerializeField] GameObject visuals;

    private Rigidbody2D rb;
    private float minimumVelocity = 0.4f;    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.velocity.magnitude > minimumVelocity)
        {
            Vector2 direction = rb.velocity.normalized;

            // Flip the sprite based on horizontal direction
            facingLeft = direction.x < 0;
            Flip();

            // Calculate the rotation angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            
            // Apply rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Reset rotation when not moving
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        
    } 

    private void Flip() {
        visuals.transform.localScale = facingLeft ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
    }
}
