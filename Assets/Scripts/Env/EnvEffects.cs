using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvEffects : MonoBehaviour
{
    [SerializeField] ParticleSystem waterSplash;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(waterSplash, collision.transform.position, Quaternion.identity);
    }
}
