using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class EnvObservator : MonoBehaviour
{
    [SerializeField] Transform[] food;
    [SerializeField] int maxFishCount = 10;
    [SerializeField] Transform[] otherFish;
    
    public void AddFoodObservations(VectorSensor sensor, Vector2 fishPos) {
        if (food.Length < 1) {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
        
        float minDist = Vector2.Distance(fishPos, food[0].localPosition);
        int index = 0;

        for (int i = 1; i < food.Length; i++) {
            float dist = Vector2.Distance(fishPos, food[i].localPosition);
            
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        sensor.AddObservation(food[index].localPosition.x);
        sensor.AddObservation(food[index].localPosition.y);
    }

    public void AddFishObservations(VectorSensor sensor, Vector2 fishPos) {
        for(int i = 0; i < maxFishCount; i++){
            if (i < otherFish.Length && otherFish[i] != null && otherFish[i].gameObject != null) {
                sensor.AddObservation(otherFish[i].localPosition.x);
                sensor.AddObservation(otherFish[i].localPosition.y);
            } else {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }
        }
    }

    public void MoveFoodTarget(Transform _food) {
        _food.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
    }

    public void MoveAllFoodTargets() {
        foreach(Transform f in food){
            if (f == null) continue;

            f.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        }
    }
}
