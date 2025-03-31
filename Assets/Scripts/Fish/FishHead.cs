using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] FishAgent fishAgent; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food)) {
            fishAgent.AddReward(1f);
            fishAgent.envObservator.RemoveFood(food.transform);
        }

        if (collision.CompareTag("Wall")) {
            fishAgent.AddReward(-1f);
            // fishAgent.WaterColor(false, Color.grey);
            // fishAgent.EndEpisode();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) {
            fishAgent.AddReward(-0.05f);
            // fishAgent.WaterColor(false, Color.grey);
        }
    }

}
