using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] FishAgent fishAgent; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food)) {
            fishAgent.AddReward(2f);
            fishAgent.WaterColor(true, Color.blue);
            fishAgent.EndEpisode();
            // fishAgent.MoveFoodTarget();
        }

        if (collision.CompareTag("Wall")) {
            fishAgent.AddReward(-1f);
            fishAgent.WaterColor(false, Color.grey);
            fishAgent.EndEpisode();
        }
    }

}
