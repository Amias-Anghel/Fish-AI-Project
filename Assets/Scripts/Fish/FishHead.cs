using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] private SwimAgent fishAgent;
    [SerializeField] private AttackAgent attackAgent;

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
                fishAgent.AddReward(-1f);
                fishAgent.EndEpisode();
            }
        }

        if (collision.CompareTag("Fish"))
        {
            if (fishAgent.isTraining)
            {
                fishAgent.envObservator.MovePupetFish();
                if (attackAgent.GetAttackDecision())
                {
                    fishAgent.AddReward(1f);
                }
                else
                {
                    fishAgent.AddReward(-1f);
                }
                fishAgent.EndEpisode();
            }

            // give damage to other fish
        }
        
    }
}
