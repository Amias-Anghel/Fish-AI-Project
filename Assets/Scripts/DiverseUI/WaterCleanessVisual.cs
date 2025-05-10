using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterCleanessVisual : MonoBehaviour
{
    [SerializeField] private Slider waterCleaness;
    [SerializeField] public EnvController envController;

    void Update()
    {
        waterCleaness.value = 1 - envController.GetWaterCleaness();
    }
}
