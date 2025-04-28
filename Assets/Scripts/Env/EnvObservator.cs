using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class EnvObservator : MonoBehaviour
{
    [SerializeField] List<Transform> food;
    [SerializeField] List<Transform> plantFood;
    [SerializeField] List<Transform> otherFish;

    [SerializeField] public EnvController envController;
    [SerializeField] public UserLimits userLimits;

    /* Add observations for closest food position (from user) or (0,0)
        and a boolean showing if food exists*/
    public void AddFoodObservations(VectorSensor sensor, Vector2 fishPos) {
        if (food.Count < 1) {
            sensor.AddObservation(false);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            return;
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

        sensor.AddObservation(true);
        sensor.AddObservation(food[index].localPosition.x);
        sensor.AddObservation(food[index].localPosition.y);
    }

    /* Add observations for closest fish position or (0,0)
        and a boolean showing if fish exists*/
    public void AddFishObservations(VectorSensor sensor, GameObject fish, Vector2 fishPos) {
        if (otherFish.Count < 2) {
            sensor.AddObservation(false);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            return;
        }

        float minDist = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < otherFish.Count; i++) {
            if (otherFish[i] == fish) continue;

            float dist = Vector2.Distance(fishPos, otherFish[i].position);
            
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        sensor.AddObservation(true);
        sensor.AddObservation(otherFish[index].localPosition.x);
        sensor.AddObservation(otherFish[index].localPosition.y);
    }

    public void AddFoodToList(Transform _food) {
        food.Add(_food);
    }

    public void RemoveFood(Transform _food) {
        if (!food.Contains(_food)) return;
        
        food.Remove(_food);
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
