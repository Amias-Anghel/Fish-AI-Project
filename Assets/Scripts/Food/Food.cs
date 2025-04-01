using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private UserLimits userLimits;
    private Rigidbody2D rb;

    void Start()
    {
        userLimits = FindObjectOfType<UserLimits>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (userLimits.IsInLimits(transform.position)) {
            rb.gravityScale = 5f;
        } else {
            rb.gravityScale = 0.05f;
        }
    }
}
