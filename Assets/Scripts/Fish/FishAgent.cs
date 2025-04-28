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
            // AddReward((2 * stress - 1) * 0.001f);
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

    private void CheckSwimPosition() {
        swimLocationTimer += Time.deltaTime;
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);

        float swimDestDist = 5f;
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);
   
        if (distToDest < swimDestDist) {
            if (isTraining){
                AddReward(1 - hunger);
                EndEpisode();
            }
        }

        if (swimLocationTimer >= 20f || distToDest < swimDestDist) {
            swimLocationTimer = 0;
            swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
        }
    }

    private void SetPoopTimer() {
        if (Time.time >= poopTimer && hunger < 0.7) {
            poopTimer = Time.time + Random.Range(20f, 40f);
            poopCounter = Random.Range(10, 20);
            // Debug.Log("will poop " + poopCounter);
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
