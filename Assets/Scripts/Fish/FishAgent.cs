using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Net.Sockets;

public class FishAgent : Agent
{
    [SerializeField] private float thrustForce = 30f;
    [SerializeField] private float rotationSpeed = 50f;
    
    private Rigidbody2D rb;

    [SerializeField] Transform targetTransform;
    [SerializeField] SpriteRenderer waterColor;
    private Color waterColor_color;

    float distToFood;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        waterColor_color = waterColor.color;
    }

    public override void OnEpisodeBegin()
    {
        // transform.localPosition = new Vector3(Random.Range(-40f, 40f), Random.Range(-20f, 15f), 0);
        // transform.localPosition = Vector3.zero;
        MoveFoodTarget();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.y);
        
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(0);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotateInput  = actions.ContinuousActions[0];
        float thrustInput  = actions.ContinuousActions[1];

        rb.angularVelocity = -rotateInput * rotationSpeed;
        Vector2 thrustDirection = transform.up;
        rb.AddForce(thrustDirection * thrustInput * thrustForce);

        if (thrustInput < 0) {
            AddReward(-0.01f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void MoveFoodTarget() {
        targetTransform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        distToFood = Vector2.Distance(transform.position, targetTransform.position);
    }

    public void WaterColor(bool normal_Color, Color color) {
        waterColor.color = normal_Color ? waterColor_color : color;
    }
}
