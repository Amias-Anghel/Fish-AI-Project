using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishStatsVisual : MonoBehaviour
{
    public bool isTraining = false;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider hungerThreshold;
    [SerializeField] private Slider stressSlider;
    [SerializeField] private Slider stressThreshold;
    [SerializeField] private Slider healthSlider;
    [SerializeField] public SwimAgent swimAgent;
    [SerializeField] public AttackAgent attackAgent;
    [SerializeField] private GameObject attackingIndicator;

    void Update()
    {
        if (swimAgent == null && !isTraining)
        {
            gameObject.SetActive(false); 
        }

        if (swimAgent != null)
        {
            transform.position = swimAgent.transform.position + new Vector3(0, 5f, 0);
            hungerSlider.value = swimAgent.GetHunger();
            hungerThreshold.value = swimAgent.GetHungerThreshold();
        }

        if (attackAgent != null)
        {
            stressSlider.value = attackAgent.GetStress();
            stressThreshold.value = attackAgent.GetAgresivityThreshold();
            attackingIndicator.GetComponent<Image>().color = attackAgent.GetAttackDecision() ? Color.red : Color.green;
            healthSlider.value = attackAgent.GetHealth();
        }
    }
}
