using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] private FishAgent fishAgent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food))
        {
            food.IsEaten(fishAgent.envObservator, fishAgent.isTraining);
            fishAgent.Eat();
        }

        if (collision.CompareTag("Wall"))
        {
            if (fishAgent.isTraining)
            {
                fishAgent.AddReward(-0.01f);
            }
        }
        
    }
}
