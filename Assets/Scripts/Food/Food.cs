using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, IFood
{
    public bool training = false;
    public bool simulateFall = false;
    [SerializeField] private UserLimits userLimits = null;
    private Rigidbody2D rb;


    void Start()
    {
        if (userLimits == null) {
            userLimits = FindObjectOfType<UserLimits>();
        }
        rb = GetComponent<Rigidbody2D>();
    }

    public void IsEaten(EnvObservator envObservator)
    {
        if (training) {
            envObservator.MoveFoodTarget(transform);
        } else {
            envObservator.RemoveFood(transform);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (simulateFall) {
            if (userLimits.IsInUserLimits(transform.position)) {
                rb.gravityScale = 5f;
            } else {
                rb.gravityScale = 0.05f;
            }
        }
    }
}
