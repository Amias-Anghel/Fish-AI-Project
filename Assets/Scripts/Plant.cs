using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] GameObject food;
    [SerializeField] GameObject plat_part;

    private float growthTimer = 0;
    [SerializeField] private float growTime = 2f;
    private int height = 0;
    [SerializeField] private int maxHeight = 7;
    [SerializeField] private float dieTime = 5f;

    private Queue<Transform> growingQ;
    private EnvObservator envObservator;

    void Start()
    {
        maxHeight = Random.Range(7, 20);
        growTime = Random.Range(1, 3);
        growingQ = new Queue<Transform>();

        Transform child = Instantiate(plat_part, transform).transform;
        child.position = transform.position;

        growingQ.Enqueue(child);

        envObservator = FindObjectOfType<EnvObservator>();
    }


    void Update()
    {
        growthTimer += Time.deltaTime;
        if (height < maxHeight && growingQ.Count > 0) {

            if (growthTimer > growTime) {
                Grow();
                growthTimer = 0;
            }
        } else {
           if (growthTimer > dieTime) {
                envObservator.AddFoodToList(Instantiate(food, transform.position, Quaternion.identity).transform);
                Destroy(gameObject);
            }
        }
    }

    private void Grow() {
        float posx = Random.Range(-1.7f, 1.7f);
        // int isfood = Random.Range(0, 20);
        height++;

        Transform grothBase = growingQ.Dequeue();
        Vector3 pos = new Vector3(grothBase.position.x + posx, grothBase.position.y + 2, 0);
        Transform child = Instantiate(plat_part, transform).transform;
        child.position = pos;

        if (height > maxHeight / 3) {
            int add = Random.Range(0, 3);
            if (add != 0) {
                int index = Random.Range(0, transform.childCount);
                growingQ.Enqueue(transform.GetChild(index));

                return;
            }
        }
        growingQ.Enqueue(child);

    }
}
