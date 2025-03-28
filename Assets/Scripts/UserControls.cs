using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserControls : MonoBehaviour
{
    [SerializeField] EnvObservator envObservator;
    [SerializeField] GameObject plantSeed;
    [SerializeField] GameObject food;
    [SerializeField] List<GameObject> fish;
    private float waitTimer;

    private GameObject selected = null;

    void Update()
    {   
        waitTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && waitTimer > 0.3f && selected != null) {
            waitTimer = 0;
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Camera.main.transform.position.z;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;
            
            GameObject spawn = Instantiate(selected, mouseWorldPos, Quaternion.identity);

            if (spawn.TryGetComponent<Food>(out Food food)) {
                envObservator.AddFoodToList(food.transform);
            }

            if (spawn.TryGetComponent<FishAgent>(out FishAgent fish)) {
                fish.envObservator = envObservator;
                selected = null;
            }
        }
    }

    public void SelectPlantSeed() {
        selected = plantSeed;
    }

    public void SelectFood() {
        selected = food;
    }

    public void SelectFish() {
        selected = fish[Random.Range(0, fish.Count)];
    }

    public void ClearSelected() {
        selected = null;
    }
}
