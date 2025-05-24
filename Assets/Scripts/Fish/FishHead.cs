using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHead : MonoBehaviour
{
    [SerializeField] private SwimAgent swimAgent;
    [SerializeField] private AttackAgent attackAgent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food))
        {
            food.IsEaten(swimAgent.envObservator, swimAgent.isTraining);
            swimAgent.Eat();
        }

        if (collision.CompareTag("Wall"))
        {
            if (swimAgent.isTraining)
            {
                swimAgent.AddReward(-1f);
                swimAgent.EndEpisode();
            }
        }

        if (collision.CompareTag("Fish"))
        {
            if (swimAgent.isTraining)
            {
                swimAgent.envObservator.MovePupetFish();
                if (attackAgent.GetAttackDecision())
                {
                    swimAgent.AddReward(1f);
                }
                else
                {
                    swimAgent.AddReward(-1f);
                }
                swimAgent.EndEpisode();
            }
            else if (attackAgent.GetAttackDecision())
            {
                AgentsManager agentsManager = collision.transform.parent.parent.GetComponent<AgentsManager>();
                
                float leftHealth = agentsManager.attackAgent.TakeDamage(0.1f);
                if (leftHealth <= 0)
                {
                    swimAgent.Eat();
                }
            }

        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            if (!swimAgent.isTraining && attackAgent.GetAttackDecision())
            {
                AgentsManager agentsManager = collision.transform.parent.parent.GetComponent<AgentsManager>();
                
                float leftHealth = agentsManager.attackAgent.TakeDamage(0.001f);
                if (leftHealth <= 0)
                {
                    swimAgent.Eat();
                }  
            }
            
        }
        
    }
}
