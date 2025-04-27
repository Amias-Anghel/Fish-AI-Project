using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUserSimulator : MonoBehaviour
{
    // switch between food and no food randomly

    [SerializeField] private GameObject food;
    [SerializeField] private List<FishAgent> fish;
    [SerializeField] private EnvObservator envObservator;

    private float switchTimer, switchTime;


    void Start()
    {
        switchTime = UnityEngine.Random.Range(0.1f, 0.5f);
        switchTimer = switchTime;
    }

    void Update()
    {
        GiveFoodDecision();
        ShowSwimLocationFish0();
    }

    private void GiveFoodDecision() {
        if (Time.time >= switchTimer) {
            switchTime = UnityEngine.Random.Range(10f, 20f);
            switchTimer = Time.time + switchTime + Time.deltaTime;
            SwitchFoodState();
        }
    }

    private void SwitchFoodState() {
        bool giveFood = UnityEngine.Random.Range(0f, 1f) > 0.2f;

        if (giveFood && !food.activeSelf) {
            food.SetActive(true);
            envObservator.AddFoodToList(food.transform);

            foreach(FishAgent f in fish) {
                f.TrainingFoodExists(giveFood);
            }

        } else if (!giveFood && food.activeSelf){
            envObservator.RemoveFood(food.transform);
            food.SetActive(false);

            foreach(FishAgent f in fish) {
                f.TrainingFoodExists(giveFood);
            }
        }

    }

    // Show where fish swim pos is
    [SerializeField] private GameObject swimLocationVisual;

    private void ShowSwimLocationFish0() {
        Vector2 pos = fish[0].TrainingGetSwimPos();
        swimLocationVisual.transform.localPosition = pos;
    }
}
