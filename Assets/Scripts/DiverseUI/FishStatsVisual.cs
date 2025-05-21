using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishStatsVisual : MonoBehaviour
{
    public bool isTraining = false;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider stressSlider;
    [SerializeField] public FishAgent fishAgent;
    [SerializeField] public AttackAgent attackAgent;
    [SerializeField] private GameObject attackingIndicator;

    void Update()
    {
        if (fishAgent == null && !isTraining)
        {
            gameObject.SetActive(false); 
        }
        
        if (fishAgent != null)
        {
            transform.position = fishAgent.transform.position + new Vector3(0, 2f, 0);
            hungerSlider.value = fishAgent.GetHunger();
        }

        if (attackAgent != null)
        {
            stressSlider.value = attackAgent.GetStress();
            attackingIndicator.GetComponent<Image>().color = attackAgent.GetAttackDecision() ? Color.red : Color.green;
        }
    }
}
