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

    void OnMouseDrag()
    {
        if (!spawnNew) return;
        spawnNew = false;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;
        
        spawnedFish = Instantiate(fish[Random.Range(0, fish.Count)], mouseWorldPos, Quaternion.identity);
        spawnedFish.GetComponent<FishAgent>().envObservator = envObservator;
        spawnedFish.GetComponent<FishAgent>().enabled = false;
        spawnedFish.transform.GetChild(0).Find("body").gameObject.SetActive(false);
        spawnedFish.GetComponent<Rigidbody2D>().gravityScale = 5f;
    }

    void OnMouseUp()
    {
        spawnNew = true;

        if (!userLimits.IsInLimits(spawnedFish.transform.position)) {
            Destroy(spawnedFish);
        }
    }
   
    void Update()
    {
        if (!spawnNew) {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;

            spawnedFish.transform.position = mouseWorldPos;
        }
    }
}
