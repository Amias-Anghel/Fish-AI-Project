using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private UserLimits userLimits;
    private Rigidbody2D rb;

    // TO DO: decoment for game, comment for training

    // void Start()
    // {
    //     userLimits = FindObjectOfType<UserLimits>();
    //     rb = GetComponent<Rigidbody2D>();
    // }

    // void Update()
    // {
    //     if (userLimits.IsInUserLimits(transform.position)) {
    //         rb.gravityScale = 5f;
    //     } else {
    //         rb.gravityScale = 0.05f;
    //     }
    // }
}
