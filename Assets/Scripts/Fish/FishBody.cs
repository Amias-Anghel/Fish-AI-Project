using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBody : MonoBehaviour
{
    [SerializeField] FishAgent fishAgent; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food)) {
            fishAgent.AddReward(-0.2f);
        }

        if (collision.TryGetComponent<Wall>(out Wall wall)) {
            fishAgent.AddReward(-1f);
            fishAgent.WaterColor(false, Color.grey);
            fishAgent.EndEpisode();
        }
    }
}
