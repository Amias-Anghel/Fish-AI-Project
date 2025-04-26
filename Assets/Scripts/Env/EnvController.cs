using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer water;
    [SerializeField] private Color cleanWater;
    [SerializeField] private Color dirtyWater;

    private float waterCleanLevel = 1f;
    private float waterDirtySpeed = 0.01f;
    
    void Update()
    {
        waterCleanLevel -= waterDirtySpeed * Time.deltaTime;
        waterCleanLevel = waterCleanLevel < 0 ? 0 : waterCleanLevel;

        Color waterColor = Color.Lerp(dirtyWater, cleanWater, waterCleanLevel);
        water.color = waterColor;
    }

    public void CleanWater() {
        waterCleanLevel = 1;
        waterDirtySpeed = 0.01f;
    }

    public void SpeedUpDirtyWater(float additionalDirtiness) {
        waterDirtySpeed += additionalDirtiness;
    }

    public void SpeedDownDirtyWater(float additionalDirtiness) {
        waterDirtySpeed -= additionalDirtiness;
    }

    public void ClearWater(float clearFactor) {
        waterCleanLevel += clearFactor;
        waterCleanLevel = waterCleanLevel > 1 ? 1 : waterCleanLevel;
    }

}
