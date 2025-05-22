using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUserSimulator : MonoBehaviour
{
    // switch between food and no food randomly

    [SerializeField] private GameObject food;
    [SerializeField] private FishAgent fish;
    [SerializeField] private EnvObservator envObservator;

    private float switchTimer, switchTime;


    void Start()
    {
        switchTime = UnityEngine.Random.Range(10f, 20f);
        switchTimer = switchTime;
        waterCleanTime = UnityEngine.Random.Range(1f, 50f);
        waterCleanTimer = waterCleanTime;
    }

    void Update()
    {
        GiveFoodDecision();
        CheckWaterCleanTime();
        ShowSwimLocationFish0();
    }

    private void GiveFoodDecision() {
        if (Time.time >= switchTimer) {
            switchTime = 15f;
            switchTimer = Time.time + switchTime + Time.deltaTime;
            SwitchFoodState();
        }
    }

    private void SwitchFoodState() {
        bool giveFood = UnityEngine.Random.Range(0f, 1f) > 0.6f;
        //bool giveFood = !food.activeSelf;
        // bool giveFood = false;
        
        if (giveFood && !food.activeSelf)
        {
            food.SetActive(true);
            envObservator.AddFoodToList(food.transform);
        }
        else if (!giveFood && food.activeSelf)
        {
            envObservator.RemoveFood(food.transform);
            food.SetActive(false);
        }

    }

    // controll water cleaness
    private float waterCleanTimer, waterCleanTime;
    private void CheckWaterCleanTime() {
        if (Time.time >= waterCleanTimer) {
            waterCleanTime = UnityEngine.Random.Range(1f, 50f);
            waterCleanTimer = Time.time + waterCleanTime + Time.deltaTime;
            ClearWater();
        }
    }

    private void ClearWater() {
        float clear = UnityEngine.Random.Range(0f, 1f);
        envObservator.envController.ClearWater(clear);
    }

    // Show where fish swim pos is
    [SerializeField] private GameObject swimLocationVisual;

    private void ShowSwimLocationFish0() {
        Vector2 pos = fish.TrainingGetSwimPos();
        swimLocationVisual.transform.localPosition = pos;
    }
}
