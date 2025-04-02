using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class EnvObservator : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;
    [SerializeField] List<Transform> food;
    [SerializeField] int maxFishCount = 10;
    [SerializeField] List<Transform> otherFish;
    [SerializeField] int foodTrainingCount = 3;

    [SerializeField] public UserLimits userLimits;

    /* Returns true if observations for food position have been added */
    public bool AddFoodObservations(VectorSensor sensor, Vector2 fishPos) {
        if (food.Count < 1) {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            return false;
        }
        
        float minDist = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < food.Count; i++) {
            float dist = Vector2.Distance(fishPos, food[i].position);
            
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        sensor.AddObservation(food[index].localPosition.x);
        sensor.AddObservation(food[index].localPosition.y);

        return true;
    }

    public void AddFishObservations(VectorSensor sensor) {
        // for(int i = 0; i < maxFishCount; i++){
        //     if (i < otherFish.Count && otherFish[i] != null && otherFish[i].gameObject != null) {
        //         sensor.AddObservation(otherFish[i].localPosition.x);
        //         sensor.AddObservation(otherFish[i].localPosition.y);
        //     } else {
        //         sensor.AddObservation(0);
        //         sensor.AddObservation(0);
        //     }
        // }
        for(int i = 0; i < maxFishCount; i++){
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(false);
        }
    }

    public void AddFoodToList(Transform _food) {
        food.Add(_food);
    }

    public void RemoveFood(Transform _food) {
        food.Remove(_food);
        Destroy(_food.gameObject);
    }

    public void MoveFoodTarget(Transform _food) {
        _food.position = userLimits.GetPositionInAquarium();
    }

    public void MoveAllFoodTargets() {
        foreach(Transform f in food){
            if (f == null) continue;

            f.position =  userLimits.GetPositionInAquarium();
        }
    }
}
