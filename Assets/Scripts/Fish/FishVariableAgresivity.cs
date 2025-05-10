using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishVariableAgresivity : MonoBehaviour, IFishBehaviour
{
    [SerializeField] private FishAgent fishAgent;

    public void CollidedWith(GameObject entity) {
        if (entity.TryGetComponent<Food>(out Food food)) {
            food.IsEaten(fishAgent.envObservator, fishAgent.isTraining);
            fishAgent.Eat();
        }

        if (entity.CompareTag("Wall")) {
            if (fishAgent.isTraining) {
                // fishAgent.AddReward(-1f);
                // fishAgent.EndEpisode();
            }
        }
    }
}
