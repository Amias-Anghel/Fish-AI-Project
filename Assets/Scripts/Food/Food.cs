using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, IFood
{
    [SerializeField] private GameObject spoiledFood;
    float spoilTimer;

    public bool simulateFall = false;
    [SerializeField] private UserLimits userLimits = null;
    private Rigidbody2D rb;


    void Start()
    {
        if (userLimits == null) {
            userLimits = FindObjectOfType<UserLimits>();
        }
        rb = GetComponent<Rigidbody2D>();

        if (!simulateFall && rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        spoilTimer = Time.time + Random.Range(30f, 60f);
    }

    public void IsEaten(EnvObservator envObservator, bool training)
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

            if (Time.time >= spoilTimer) {
                // spoil
                FindObjectOfType<EnvObservator>().RemoveFood(transform);
                Instantiate(spoiledFood, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
