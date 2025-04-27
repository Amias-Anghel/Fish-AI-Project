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
    [SerializeField] public Transform head;

    private float health, stress, age;
    private float hunger;

    private Vector2 swimLocation;
    private float swimLocationTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
    }

    public override void OnEpisodeBegin()
    {
        envObservator.MoveAllFoodTargets();

        age = 0;
        health = 0;
        hunger = 1;
        stress = 0;
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

        // food position - 3
        envObservator.AddFoodObservations(sensor, headRelativePos);

        // target fish position - 3 -- only if should attack decision is made
        // to add at stress training 
        // envObservator.AddFishObservations(sensor, gameObject, headRelativePos);

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
        bool shouldAttack = actions.DiscreteActions[0] == 1;

        if (shouldAttack)
        {
            // debug attack
            // if attacking, send as food destination the location of closest fish
            // to do after food training, together with stress training
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
        // hunger
        hunger += Time.deltaTime * 0.1f;
        hunger = hunger > 1 ? 1 : hunger;

        // swiming
        CheckSwimPosition();
        AddReward(-0.001f);

        // stress
        ComputeStress();
    }

    private void ComputeStress() {
        float evnwater = envObservator.envController.GetWaterCleaness();
        float hungerWeight = 0.55f;
        float waterWeight = 0.45f;
        float stress_ = Mathf.Clamp01((hunger * hungerWeight) + ((1 - evnwater) * waterWeight));
        // Debug.Log("stress: " + stress_ + " hunger: " + hunger + " water: " + evnwater);
    }

    public void Eat() {
        if (isTraining) {
            AddReward(Mathf.Pow(hunger, 2));
            // AddReward(1f);
            // Debug.Log("food reward! " + hunger);
            // EndEpisode();
        }

        hunger -= 0.5f;
        hunger = hunger < 0 ? 0 : hunger;
    }

    private void CheckSwimPosition() {
        swimLocationTimer += Time.deltaTime;
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);

        float swimDestDist = 10f;
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);
        // Debug.Log(distToDest);

        Debug.Log(foodExists);
        if (distToDest < swimDestDist) {
            if (!foodExists) {
                AddReward(0.5f);
            //     EndEpisode();
            } else {
                AddReward(Mathf.Pow(1 - hunger, 2));
            }
        }

        if (swimLocationTimer >= 20f || distToDest < swimDestDist) {
            swimLocationTimer = 0;
            swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
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
