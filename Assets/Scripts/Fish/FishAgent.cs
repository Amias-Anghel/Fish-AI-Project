using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FishAgent : Agent
{
    [SerializeField] private AgentsManager agentsManager;
    private FishVisuals fishVisuals;
    public bool isTraining = true;

    private float movementSpeed = 30f;
    private Rigidbody2D rb;

    [SerializeField] public EnvObservator envObservator;
    [SerializeField] public Transform head;

    // stats
    private float hunger;
    private bool hasTarget = true;

    // swiming around
    private Vector2 swimLocation;
    private float swimLocationTimer;

    /* 
        total reward = 1:

        sum((1-goalProcent) * stepRewardMultiplier) + finalreward = 1
        goalProcentStep = 0.1f: n = 1 - 4.4x  (n)finalreward = 0.56, (x)stepRewardMultiplier = 0.1
        goalProcentStep = 0.05f: n = 1 - 8.25x  (n)finalreward = 0.175, (x)stepRewardMultiplier = 0.1

        total reward = 0.7:
        goalProcentStep = 0.05f: n = 0.7 - 8.25x  (n)finalreward = 0.2875, (x)stepRewardMultiplier = 0.05
    */
    float swimGoalProcent = 0.8f; // calc goal distance for swim as swimprocent from maxdist
    float goalProcentStep = 0.05f; // how much the procent lowers per goal reached
    float stepRewardMultiplier = 0.05f; // goal step reward weight
    float stashedReward = 0f; // total goal that is already reched from starting position
    float finalReward = 0.2875f; // reward for reaching swim position
    float maxDist = 150;

    

    void Start()
    {
        fishVisuals = GetComponent<FishVisuals>();
        rb = GetComponent<Rigidbody2D>();
        swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());

        hunger = Random.Range(0f, 1f);
    }

    public override void OnEpisodeBegin()
    {
        if (isTraining) {
            envObservator.MoveAllFoodTargets();
            hunger = Random.Range(0f, 1f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    { 
        // fish position - 2
        // sensor.AddObservation(transform.localPosition.x);
        // sensor.AddObservation(transform.localPosition.y);

        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);
        sensor.AddObservation(headRelativePos.x);
        sensor.AddObservation(headRelativePos.y);

        // swim position - 2
        sensor.AddObservation(swimLocation.x);
        sensor.AddObservation(swimLocation.y);


        // bool attack = agentsManager.attackAgent == null ? false : agentsManager.attackAgent.GetAttackDecision();

        // if (attack)
        // {
        //     //target fish position - 3 -- only if should attack decision is made
        //     hasTarget = envObservator.AddFishObservations(sensor, gameObject, headRelativePos);
        // }
        // else
        // {
            // food position - 3
            //hasTarget = envObservator.AddFoodObservations(sensor, headRelativePos);
        // }
        sensor.AddObservation(hasTarget);

        // fish hunger
        sensor.AddObservation(hunger);

        // Total: 2 2 3 1 = 8
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 movement;
        movement.x = actions.ContinuousActions[0];
        movement.y = actions.ContinuousActions[1];

        rb.velocity = movement * movementSpeed;

        if (rb.velocity.magnitude > 0.1f)
            fishVisuals.FlipAndRotate();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    void Update()
    {
        if (!isTraining) {
            // hunger
            hunger += Time.deltaTime * 0.05f;
            hunger = hunger > 1 ? 1 : hunger;
        }

        // swiming
        CheckSwimPosition();

        if (isTraining) {
            AddReward(-0.001f);
        }
    }

    public void Eat() {
        if (isTraining) {
            // AddReward(hunger);
            if (hunger >= 0.5f) AddReward(1);
            EndEpisode();
        }
        fishVisuals.SetPoopTimer();
        hunger -= 0.5f;
        hunger = hunger < 0 ? 0 : hunger;
    }

    private void CheckSwimPosition() {
        swimLocationTimer += Time.deltaTime;
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);

        float swimDestDist = 5f;
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);


        if (isTraining){
            if (hasTarget) {
                if (distToDest < swimDestDist) {
                    // AddReward(1 - hunger);
                    if (hunger < 0.5f) AddReward(1);
                    EndEpisode();
                }
            } 
            else { 
                // calculate new proximity reward and swimGoalProcent
                float targetDist = maxDist * swimGoalProcent;
                if (distToDest < targetDist) {
                    float reward = (1 - swimGoalProcent) * stepRewardMultiplier;
                    AddReward(reward);
                    // Debug.Log("hit reward " + reward + " at procent " + swimGoalProcent);
                    swimGoalProcent = Mathf.Max(0, swimGoalProcent - goalProcentStep);
                }

                if (distToDest < swimDestDist) {
                    // Debug.Log("reach reward: " + (stashedReward + 0.56) + " from which, stashed: " + stashedReward);
                    AddReward(finalReward + stashedReward);
                    EndEpisode();
                }
            }
        }

        // select new swim location
        if (swimLocationTimer >= 20f || distToDest < swimDestDist) {
            swimLocationTimer = 0;
            swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
            SetSwimGoalData();
        }
    }

    /* 
        Sets the stashed reward for proximity to swim goal
        Sets the swimGoalProcent for next proximity goals
        Called only on new episode or when goal changes
    */
    private void SetSwimGoalData() {
        swimGoalProcent = 0.8f;
        stashedReward = 0f;

        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);
        float compareDist = maxDist * swimGoalProcent;

        while (distToDest < compareDist && swimGoalProcent > 0) {
            float reward = (1 - swimGoalProcent) * stepRewardMultiplier;
            stashedReward += reward;
            swimGoalProcent = Mathf.Max(0, swimGoalProcent - goalProcentStep);

            compareDist = maxDist * swimGoalProcent;
        }

        // Debug.Log("New Episode: stashedReward: " + stashedReward + " procent: " + swimGoalProcent);
    }

    public Vector2 TrainingGetSwimPos() {
        return swimLocation;
    }

    public float GetHunger() {
        return hunger;
    }
}