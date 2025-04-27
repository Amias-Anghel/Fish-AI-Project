using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] GameObject plant_food;
    [SerializeField] GameObject plant_part;
    [SerializeField] GameObject plant_dead;

    private float growthTimer = 0;
    [SerializeField] private float growTime = 2f;
    [SerializeField] private float dieTime = 0.5f;

    private EnvObservator envObservator;
    private UserLimits userLimits;

    private Queue<Vector2> growingQ;
    private Queue<GameObject> dieQ;
    private string growModel;
    private Vector2 currentPos;
    private int currentIndex;

    void Start()
    {
        growTime = Random.Range(0.1f, 0.3f);
        growingQ = new Queue<Vector2>();
        dieQ = new Queue<GameObject>();

        envObservator = FindObjectOfType<EnvObservator>();
        userLimits = FindObjectOfType<UserLimits>();

        currentPos = transform.position;
        currentIndex = 0;

        growModel = CreateGrowModel();
    }


    void Update()
    {
        if (currentIndex < growModel.Length && Time.time >= growthTimer) {
            GrowPlantByModel(growModel, currentIndex);
            currentIndex++;
            growthTimer = Time.time + growTime;

            if (currentIndex == growModel.Length) {
                growthTimer = Time.time + dieTime;
            }
            
        }
        else if (currentIndex >= growModel.Length && Time.time >= growthTimer) {
            // die
            PlantRezidue();
            Destroy(gameObject);
        }
    }

    private string CreateGrowModel() {
        int proccesNR = 3;
        string result = "X";
        string model = GetRandomModel();
        
        for (int i = 0; i < proccesNR; i++) {
            result = ProccesModelString(result, model);
        }

        return result;
    }

    private string GetRandomModel() {
        string[] models = {
             "n[SnFX]DnXDf",  "n[SnFX]Dn[X]Df", "n[FnFnSnXF]Dn[X]Df", "n[FnFnSnXF]Dn[X]Df",
             "n[FnSn[X]FnDn]", "n[FnSn[XSn]Fn[DnX]]", "n[FnSn[XSf]Fn[DnX]]"
        };

        return models[Random.Range(0, models.Length)];
    }

    /*
        S - stanga
        D - dreapta
        F - sus
        f - food
        n - frunza
        [] - queue
        X - repeat
    */
    private string ProccesModelString(string initialStr, string model) {
        StringBuilder result = new StringBuilder();

        foreach (char c in initialStr) {
            if (c == 'X') {
                result.Append(model);
            } else {
                result.Append(c);
            }

        }

        return result.ToString();
    }

    private void GrowPlantByModel(string model, int index) {
        float visualSize = 2f;
        
        char c = model[index];
        // foreach (char c in model) {
            switch(c) {
                case 'S':
                    currentPos += new Vector2(-visualSize, visualSize);
                    break;
                case 'D':
                    currentPos += new Vector2(visualSize, visualSize);
                    break;
                case 'F':
                    currentPos += new Vector2(0, visualSize);
                    break;
                case 'f':
                    if (!userLimits.IsInAquariumLimits(currentPos)) break;
                    dieQ.Enqueue(Instantiate(plant_food, currentPos, Quaternion.identity));
                    break;
                case 'n':
                    if (!userLimits.IsInAquariumLimits(currentPos)) break;
                    dieQ.Enqueue(Instantiate(plant_part, currentPos, Quaternion.identity));
                    break;
                case '[':
                    growingQ.Enqueue(currentPos);
                    break;
                case ']':
                    currentPos = growingQ.Dequeue();
                    break;
            }
        // }
    }

    private void PlantRezidue(){
        while (dieQ.Count > 0) {
            GameObject part = dieQ.Dequeue();
            if (part == null) continue;
            Instantiate(plant_dead, part.transform.position, Quaternion.identity);
            Destroy(part);
        }
    }

}
