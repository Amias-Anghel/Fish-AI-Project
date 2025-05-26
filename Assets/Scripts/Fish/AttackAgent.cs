using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AttackAgent : Agent
{
    public bool isTraining = false;
    private bool attackDecision;
    [Range(0f, 1f)][SerializeField] private float stress;

    [SerializeField] private AgentsManager agentsManager;

    [Range(0f, 1f)] public float agresivityThreshold = 0.5f;

    private float health = 1f;

    void Start()
    {
        ComputeStress();
    }

    public void SetRandom()
    {
        agresivityThreshold = Random.Range(0, 1f);
    }

    public override void OnEpisodeBegin()
    {
        if (isTraining)
        {
            stress = Random.Range(0, 1f);
            agresivityThreshold = Random.Range(0, 1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Attack decision (discrete)
        attackDecision = actions.DiscreteActions[0] == 1;
        ComputeAttackReward();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        bool attack = Input.GetKey(KeyCode.Space);

        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = attack ? 1 : 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(stress);
        sensor.AddObservation(agresivityThreshold);
    }

    private void ComputeAttackReward()
    {
        float attackReward = 0;
        if (stress < agresivityThreshold)
        {
            attackReward = -1;
        }
        else
        {
            attackReward = 1;
        }

        if (!attackDecision)
        {
            attackReward *= -1;
        }

        if (isTraining)
        {
            AddReward(attackReward);
            EndEpisode();
        }
    }

    void Update()
    {
        if (!isTraining)
        {
            ComputeStress();
           
            if (health <= 0)
            {
                agentsManager.Die();
            }
        }
    }

    private void ComputeStress()
    {
        float evnwater = agentsManager.swimAgent.envObservator.envController.GetWaterCleaness();
        float hunger = agentsManager.swimAgent.GetHunger();
        float hungerThreshold = agentsManager.swimAgent.GetHungerThreshold();

        hunger *= 1f - hungerThreshold;

        float hungerWeight = 0.55f;
        float waterWeight = 0.45f;
        stress = Mathf.Clamp01((hunger * hungerWeight) + ((1 - evnwater) * waterWeight));
    }

    public bool GetAttackDecision()
    {
        return attackDecision;
    }

    public float GetStress()
    {
        return stress;
    }
    public float GetAgresivityThreshold()
    {
        return agresivityThreshold;
    }

    public float GetHealth()
    {
        return health;
    }

    public float TakeDamage(float dmg = 0.05f)
    {
        health -= dmg;

        return health;
    }
}
