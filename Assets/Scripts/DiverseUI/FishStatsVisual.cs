using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishStatsVisual : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private FishAgent fishAgent;

    void Update()
    {
        transform.position = fishAgent.transform.position + new Vector3(0, 2f, 0);
        slider.value = fishAgent.GetHunger();
    }
}
