using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodContainer : MonoBehaviour {
    public bool isFood;
    [SerializeField] private EnvObservator envObservator;
    [SerializeField] private GameObject foodPrefab;

    [SerializeField] private Transform[] spawnPoints;

    private UserLimits userLimits;

    private Vector2 initialPos;
    private bool rotated, inLimits;
    private float spawnTime;

    void Start()
    {
        initialPos = transform.position;
        userLimits = FindObjectOfType<UserLimits>();
    }

    void OnMouseUp()
    {
        if (rotated) {
            transform.Rotate(0, 0, 180);
            rotated = false;
        }

        inLimits = false;
        transform.position = initialPos;
    }

    void OnMouseDrag()
    {
        inLimits = userLimits.IsInUserLimits(transform.position);
        if (!rotated && inLimits) {
            transform.Rotate(0, 0, 180);
            rotated = true;
        } else if (rotated && !inLimits) {
            transform.Rotate(0, 0, 180);
            rotated = false;
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        transform.position = mouseWorldPos;
    }

    void Update()
    {
        spawnTime += Time.deltaTime;

        if (inLimits && spawnTime > 0.3f) {
            spawnTime = 0;
            SpawnFood();
        }
    }

    private void SpawnFood() {
        int index = Random.Range(0, spawnPoints.Length);

        GameObject spawn = Instantiate(foodPrefab, spawnPoints[index].position, Quaternion.identity);
        if (isFood) envObservator.AddFoodToList(spawn.transform);    
    }
}
