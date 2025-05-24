using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnButton : MonoBehaviour
{
    [SerializeField] List<GameObject> fish;
    private UserLimits userLimits;
    private EnvObservator envObservator;

    private GameObject spawnedFish;
    bool spawnNew = true;

    void Awake()
    {
        envObservator = FindObjectOfType<EnvObservator>();
        userLimits = FindObjectOfType<UserLimits>();
    }

    void OnMouseDown()
    {
        if (!spawnNew) return;
        spawnNew = false;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        spawnedFish = Instantiate(fish[Random.Range(0, fish.Count)], mouseWorldPos, Quaternion.identity);
        spawnedFish.transform.SetParent(envObservator.gameObject.transform);
        spawnedFish.GetComponent<SwimAgent>().envObservator = envObservator;
        spawnedFish.transform.GetChild(1).Find("fall").GetComponent<FishFall>().SetFall();
    }

    void OnMouseDrag()
    {
        if (!spawnNew) {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;

            spawnedFish.transform.position = mouseWorldPos;
        }
    }

    void OnMouseUp()
    {
        spawnNew = true;

        if (!userLimits.IsInUserLimits(spawnedFish.transform.position))
        {
            Destroy(spawnedFish);
        }
        else
        {
            envObservator.AddFishToList(spawnedFish.transform);
        }
    }
}
