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

    /* Returns true if observations for food position have been added */
    public bool AddFoodObservations(VectorSensor sensor, Vector2 fishPos) {
        if (food.Count < 1) {
            return false;
        }
        
        float minDist = Vector2.Distance(fishPos, food[0].localPosition);
        int index = 0;

        for (int i = 1; i < food.Count; i++) {
            float dist = Vector2.Distance(fishPos, food[i].localPosition);
            
            if (dist < minDist) {
                minDist = dist;
                index = i;
            }
        }

        sensor.AddObservation(food[index].localPosition.x);
        sensor.AddObservation(food[index].localPosition.y);

        return true;
    }

    public void AddFishObservations(VectorSensor sensor, Vector2 fishPos) {
        // for(int i = 0; i < maxFishCount; i++){
        //     if (i < otherFish.Count && otherFish[i] != null && otherFish[i].gameObject != null) {
        //         sensor.AddObservation(otherFish[i].localPosition.x);
        //         sensor.AddObservation(otherFish[i].localPosition.y);
        //     } else {
        //         sensor.AddObservation(0);
        //         sensor.AddObservation(0);
        //     }
        // }

        sensor.AddObservation(0);
        sensor.AddObservation(0);
    }

    public void AddFoodToList(Transform _food) {
        food.Add(_food);
    }

    public void RemoveFood(Transform _food) {
        food.Remove(_food);
        Destroy(_food.gameObject);
    }

    public void MoveFoodTarget(Transform _food) {
        _food.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
    }

    public void SpawnFood() {
        MoveAllFoodTargets();

        while (food.Count < foodTrainingCount) {
            Vector3 pos = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
            Transform f = Instantiate(foodPrefab, pos, Quaternion.identity).transform;
            food.Add(f);
        } 
    }

    private void MoveAllFoodTargets() {
        foreach(Transform f in food){
            if (f == null) continue;

            f.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        }
    }
}
