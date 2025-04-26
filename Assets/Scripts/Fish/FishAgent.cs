using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FishAgent : Agent
{
    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float lifeExpentancy = 30f;
    
    private Rigidbody2D rb;

    [SerializeField] public EnvObservator envObservator;
    [SerializeField] public Transform head;

    private float health, hunger, stress, age;

    private Vector2 swimLocation;
    private float swimLocationTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
    }

    public override void OnEpisodeBegin()
    {
        // transform.position = envObservator.userLimits.GetPositionInAquarium(0.5f);
        envObservator.MoveAllFoodTargets();

        age = 0;
        health = 0;
        hunger = 0;
        stress = 0;
        // swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
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

        // food position - 6
        envObservator.AddFoodObservations(sensor, headRelativePos);
        envObservator.AddPlantFoodObservations(sensor, headRelativePos);

        // other fish position - 3 
        envObservator.AddFishObservations(sensor, gameObject, headRelativePos);

        // fish parameters that can change - 3
        sensor.AddObservation(health);
        sensor.AddObservation(hunger);
        sensor.AddObservation(stress);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 movement;
        movement.x = actions.ContinuousActions[0];
        movement.y = actions.ContinuousActions[1];

        rb.velocity = movement * movementSpeed;

        if (rb.velocity.magnitude > 0.1f)
            FlipAndRotate();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // void Update()
    // {
        // swimLocationTimer += Time.deltaTime;
        // Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);
        // if (swimLocationTimer >= 1f || Vector2.Distance(swimLocation, headRelativePos) < 0.2f) {
        //     swimLocationTimer = 0;
        //     swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
        // }

        // // age += Time.deltaTime;
        // // if (age > lifeExpentancy) {
        //     // Destroy(gameObject);
        //     // AddReward(0.01f);
        //     // EndEpisode();
        // // }

        // if (!envObservator.userLimits.IsInAquariumLimits(transform.position)) {
        //     transform.position = envObservator.userLimits.GetPositionInAquarium();
        // }
    // }

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
