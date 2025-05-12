using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishVariableAgresivity : MonoBehaviour, IFishBehaviour
{
    [SerializeField] private FishAgent fishAgent;
    // agresivity
    public float agresivityMaxThreshold = 0.7f;
    public float agresivityMinThreshold = 0.5f;

    public void CollidedWith(GameObject entity) {
        if (entity.TryGetComponent<Food>(out Food food)) {
            food.IsEaten(fishAgent.envObservator, fishAgent.isTraining);
            fishAgent.Eat(); 
        }

        // if (entity.CompareTag("Fish")) {
        //     if (entity.TryGetComponent<FishAgent>(out FishAgent otherFish)) {
        //         if (fishAgent.GetAttackDecision()) {
        //             // give other fish damage
        //         }
        //     }

        //     // float stageReward = ComputeAttackReward();
        //     float stageReward = 1;
        //     fishAgent.AddReward(stageReward);

        //     fishAgent.EndEpisode();
        // }

        // if (entity.CompareTag("Wall")) {
        //     if (fishAgent.isTraining) {
                // fishAgent.AddReward(-1f);
                // fishAgent.EndEpisode();
        //     }
        // }
    }

    private float ComputeAttackReward() {
        float stress = fishAgent.GetStress();
        if (stress < agresivityMinThreshold)
        {
            return -(agresivityMinThreshold - stress) / agresivityMinThreshold;
        }
        else if (stress < agresivityMaxThreshold)
        {
            return (stress - agresivityMinThreshold) / (agresivityMaxThreshold - agresivityMinThreshold);
        }
        else
        {
            return 1 - ((1 - stress) / (1 - agresivityMaxThreshold));
        }
    }
}
