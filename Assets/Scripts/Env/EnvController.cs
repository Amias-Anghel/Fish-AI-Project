using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [SerializeField] private EnvObservator envObservator;

    [SerializeField] private SpriteRenderer water;
    [SerializeField] private Color cleanWater;
    [SerializeField] private Color dirtyWater;


    private float waterCleanLevel = 1f;
    private float waterDirtySpeed = 0.01f;
    private Queue<float> additionalDirtyQ;
    private float removeDirtyTimer, removeDirtyTime = 0.3f;
    


    void Update()
    {
        waterCleanLevel -= waterDirtySpeed * Time.deltaTime;
        waterCleanLevel = waterCleanLevel < 0 ? 0 : waterCleanLevel;

        Color waterColor = Color.Lerp(dirtyWater, cleanWater, waterCleanLevel);
        water.color = waterColor;
        Debug.Log(waterCleanLevel);

        // REMOVE additional dirtiness from dirty speed
        if (Time.time >= removeDirtyTimer) {
            removeDirtyTimer = Time.time + removeDirtyTime;
            SlowDirtyWater();
        }
    }

    public void CleanWater() {
        waterCleanLevel = 1;
        waterDirtySpeed = 0.01f;

        additionalDirtyQ.Clear();
    }

    public void SpeedUpDirtyWater(float additionalDirtiness) {
        waterDirtySpeed += additionalDirtiness;
        additionalDirtyQ.Enqueue(additionalDirtiness);
    }

    private void SlowDirtyWater() {
        if (additionalDirtyQ.Count < 1) return;

        float dirtyFloat = additionalDirtyQ.Dequeue();
        waterDirtySpeed -= dirtyFloat;
        waterDirtySpeed = waterDirtySpeed < 0.01f ? 0.01f : waterDirtySpeed; 
    }

}
