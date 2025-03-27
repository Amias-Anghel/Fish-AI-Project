using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserControls : MonoBehaviour
{
    [SerializeField] GameObject plantSeed;
    private float waitTimer;

    private GameObject selected = null;

    void Update()
    {   
        waitTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && waitTimer > 0.1f && selected != null) {
            waitTimer = 0;
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Camera.main.transform.position.z;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;
            
            Instantiate(selected, mouseWorldPos, Quaternion.identity);
        }
    }

    public void SelectPlantSeed() {
        selected = plantSeed;
    }

    public void ClearSelected() {
        selected = null;
    }
}
