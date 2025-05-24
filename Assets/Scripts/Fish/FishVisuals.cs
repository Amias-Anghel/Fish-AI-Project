using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishVisuals : MonoBehaviour
{
    public bool isTraining = false;

    // pooping
    [SerializeField] private GameObject fishPoop;
    float poopTimer;
    int poopCounter, spawnFreq;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        poopTimer = Time.time + Random.Range(20f, 40f);
    }

    void Update()
    {
        if (!isTraining) {
            // poop
            Poop();
        }
    }

    public void SetPoopTimer()
    {
        if (Time.time >= poopTimer)
        {
            poopTimer = Time.time + Random.Range(20f, 40f);
            poopCounter = Random.Range(10, 20);
        }
    }

    private void Poop()
    {
        // if needs to poop, poop every spawnFreq frames
        if (poopCounter > 0)
        {
            if (spawnFreq == 0)
            {
                poopCounter--;
                Instantiate(fishPoop, transform.position, Quaternion.identity);
                spawnFreq = 3;
            }
            else
            {
                spawnFreq--;
            }
        }
    }
    
    public void FlipAndRotate()
    {
        Vector2 direction = rb.velocity.normalized;
        bool faceRight = direction.x > 0;

        Vector3 scale = transform.localScale;
        scale.x = faceRight ? -1 : 1;
        transform.localScale = scale;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
