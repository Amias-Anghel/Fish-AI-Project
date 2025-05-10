using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishStatsVisual : MonoBehaviour
{
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider stressSlider;
    [SerializeField] public FishAgent fishAgent;
    [SerializeField] private GameObject attackingIndicator;

    void Update()
    {
        if (fishAgent == null) {
            gameObject.SetActive(false);
        }

        transform.position = fishAgent.transform.position + new Vector3(0, 2f, 0);
        hungerSlider.value = fishAgent.GetHunger();
        stressSlider.value = fishAgent.GetStress();
        attackingIndicator.SetActive(fishAgent.GetAttackDecision());
    }
}
