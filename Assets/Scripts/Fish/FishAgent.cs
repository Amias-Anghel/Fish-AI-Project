using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class FishAgent : Agent
{
    public bool isTraining = true;

    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float lifeExpentancy = 30f;
    
    private Rigidbody2D rb;

    [SerializeField] public EnvObservator envObservator;
    [SerializeField] private Transform head;

    // stats
    private float health, stress, age;
    private float hunger;

    private bool attackDecision;

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

    // pooping

    [SerializeField] private GameObject fishPoop;
    float poopTimer;
    int poopCounter, spawnFreq;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());

        hunger = Random.Range(0f, 1f);

        SetPoopTimer();
    }

    public override void OnEpisodeBegin()
    {
        if (isTraining) {
            envObservator.MoveAllFoodTargets();
            envObservator.MovePupetFish();

            age = 0;
            health = 0;
            hunger = Random.Range(0f, 1f);

            ComputeStress();
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

        // if (attackDecision)
        // {
            // if attacking, send as food destination the location of closest fish
            // target fish position - 3 -- only if should attack decision is made
            // hasTarget = envObservator.AddFishObservations(sensor, gameObject, headRelativePos);
        // } else {
            // food position - 3
            hasTarget = envObservator.AddFoodObservations(sensor, headRelativePos);
        // }

        // fish parameters that can change - 3
        sensor.AddObservation(hunger);
        sensor.AddObservation(stress);
        sensor.AddObservation(health);

        // Total: 2 2 3 3 = 10
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 movement;
        movement.x = actions.ContinuousActions[0];
        movement.y = actions.ContinuousActions[1];

        rb.velocity = movement * movementSpeed;

        if (rb.velocity.magnitude > 0.1f)
            FlipAndRotate();

        // Attack decision (discrete)
        attackDecision = actions.DiscreteActions[0] == 1;

        // AddAttackDecisionReward();
    }

    private void AddAttackDecisionReward() {
        // Reward/punish the decision itself
        float threshold = 0.5f;
        if (attackDecision)
        {
            if (stress > threshold)
                AddReward(+0.01f);   // good, it attacked when stressed
            else
                AddReward(-0.01f);   // bad, attacked too early
        }
        else
        {
            if (stress <= threshold)
                AddReward(+0.005f);  // OK to not attack when calm
            else
                AddReward(-0.005f);  // wrong: should have attacked
        }
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
            // stress
            ComputeStress();
            // poop
            Poop();
        }

        // swiming
        CheckSwimPosition();

        if (isTraining) {
            AddReward(-0.001f);
        }

    }

    private void ComputeStress() {
        float evnwater = envObservator.envController.GetWaterCleaness();
        float hungerWeight = 0.55f;
        float waterWeight = 0.45f;
        stress = Mathf.Clamp01((hunger * hungerWeight) + ((1 - evnwater) * waterWeight));
        // Debug.Log("stress: " + stress_ + " hunger: " + hunger + " water: " + evnwater);
    }

    public void Eat() {
        if (isTraining) {
            AddReward(hunger);
            EndEpisode();
        }
        SetPoopTimer();
        hunger -= 0.5f;
        hunger = hunger < 0 ? 0 : hunger;
    }

    private bool hasTarget;
    private void CheckSwimPosition() {
        swimLocationTimer += Time.deltaTime;
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);

        float swimDestDist = 5f;
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);


        if (isTraining){
            if (foodExists || hasTarget) {
                if (distToDest < swimDestDist) {
                    AddReward(1 - hunger);
                    EndEpisode();
                }
            } 
            else  { //if (!attackDecision || !hasTarget)
                // calculate new proximity reward and swimGoalProcent
                // float targetDist = maxDist * swimGoalProcent;
                // if (distToDest < targetDist) {
                //     float reward = (1 - swimGoalProcent) * stepRewardMultiplier;
                //     AddReward(reward);
                //     // Debug.Log("hit reward " + reward + " at procent " + swimGoalProcent);
                //     swimGoalProcent = Mathf.Max(0, swimGoalProcent - goalProcentStep);
                // }

                if (distToDest < swimDestDist) {
                    // Debug.Log("reach reward: " + (stashedReward + 0.56) + " from which, stashed: " + stashedReward);
                    // AddReward(finalReward + stashedReward);
                    AddReward(1);
                    EndEpisode();
                }
            }
        }

        // select new swim location
        if (swimLocationTimer >= 20f || distToDest < swimDestDist) {
            swimLocationTimer = 0;
            swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
            // SetSwimGoalData();
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

    private void SetPoopTimer() {
        if (Time.time >= poopTimer && hunger < 0.7) {
            poopTimer = Time.time + Random.Range(20f, 40f);
            poopCounter = Random.Range(10, 20);
        }
    }

    private void Poop() {
        // if needs to poop, poop every spawnFreq frames
        if (poopCounter > 0) {
            if (spawnFreq == 0) {
                poopCounter--;
                Instantiate(fishPoop, transform.position, Quaternion.identity);
                spawnFreq = 3; 
            }
            else {
                spawnFreq--;
            }
        }
    }

    private bool foodExists = true;
    public void TrainingFoodExists(bool exists) {
        foodExists = exists;
    }

    public Vector2 TrainingGetSwimPos() {
        return swimLocation;
    }

    public float GetHunger() {
        return hunger;
    }

    public float GetStress() {
        return stress;
    }

    public float GetAge() {
        return age;
    }

    public float GetHealth() {
        return health;
    }

    public bool GetAttackDecision() {
        return attackDecision;
    }


    private void FlipAndRotate() {
        Vector2 direction = rb.velocity.normalized;
        bool faceRight = direction.x > 0;

        Vector3 scale = transform.localScale;
        scale.x = faceRight ? -1 : 1;
        transform.localScale = scale;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    
}
