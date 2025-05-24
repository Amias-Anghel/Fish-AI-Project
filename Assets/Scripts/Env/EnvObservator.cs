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
    public bool HasFoodTarget() {
        return food.Count > 0;
    }

    public Vector2 GetClosestFoodPos(Vector2 fishPos)
    {
        float minDist = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < food.Count; i++)
        {
            float dist = Vector2.Distance(fishPos, food[i].localPosition);

            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return food[index].localPosition;
    }

    /* Add observations for closest fish position or (0,0)
        and a boolean showing if fish exists*/

    public bool HasFishTarget() {
        return otherFish.Count > 1;
    }

    public Vector2 GetClosestFishPos(Vector2 fishPos, Transform fish)
    {
        float minDist = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < otherFish.Count; i++)
        {
            if (otherFish[i] == fish) continue;

            float dist = Vector2.Distance(fishPos, otherFish[i].position);

            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        return otherFish[index].localPosition;
    }

    public void AddAquariumLimits(VectorSensor sensor)
    {
        sensor.AddObservation(userLimits.aq_up.position.y);
        sensor.AddObservation(userLimits.aq_down.position.y);
        sensor.AddObservation(userLimits.aq_left.position.x);
        sensor.AddObservation(userLimits.aq_right.position.x);
    }

    public void AddFoodToList(Transform _food)
    {
        food.Add(_food);
    }

    public void RemoveFood(Transform _food) {
        if (!food.Contains(_food)) return;
        
        food.Remove(_food);
    }
    
    public void AddFishToList(Transform _fish)
    {
        otherFish.Add(_fish);
    }

    public void RemoveFish(Transform _fish) {
        if (!otherFish.Contains(_fish)) return;
        
        otherFish.Remove(_fish);
    }

    public void MoveFoodTarget(Transform _food)
    {
        _food.position = userLimits.GetPositionInAquarium();
    }

    public void MoveAllFoodTargets() {
        foreach(Transform f in food){
            if (f == null) continue;

            MoveFoodTarget(f);
        }
    }

    public void MovePupetFish() {
        if (otherFish.Count < 2) return;
        
        otherFish[1].position = userLimits.GetPositionInAquarium();
    }
}
