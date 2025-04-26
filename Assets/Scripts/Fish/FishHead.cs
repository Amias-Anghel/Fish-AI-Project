using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] FishAgent fishAgent; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food)) {
            food.IsEaten(fishAgent.envObservator);

            if (fishAgent.isTraining) {
                fishAgent.AddReward(1f);
                fishAgent.EndEpisode();
            }
        }

        if (collision.CompareTag("Wall")) {
            if (fishAgent.isTraining) {
                fishAgent.AddReward(-1f);
                fishAgent.EndEpisode();
            }
        }
    }
}
