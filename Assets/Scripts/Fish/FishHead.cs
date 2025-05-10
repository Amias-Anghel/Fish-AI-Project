using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] private GameObject fish;
    private IFishBehaviour fishAgent;

    void Start()
    {
        fishAgent = fish.GetComponent<IFishBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        fishAgent.CollidedWith(collision.gameObject);
    }
}
