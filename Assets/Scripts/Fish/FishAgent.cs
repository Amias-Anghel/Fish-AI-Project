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
    [SerializeField] private float rotationSpeed = 200f;

    private float minimumVelocity = 0.4f;
    [SerializeField] GameObject visuals;
    private bool facingLeft = true;
    private float rotationPenaltyTimer = 0;
    

    private Rigidbody2D rb;

    [SerializeField] Transform targetTransform;
    [SerializeField] SpriteRenderer waterColor;
    private Color waterColor_color;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        waterColor_color = waterColor.color;
    }


    public override void OnEpisodeBegin()
    {
        // transform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        transform.localPosition = Vector3.zero;
        MoveFoodTarget();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

        Vector3 rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        sensor.AddObservation(rotation);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotateInput  = actions.ContinuousActions[0];
        float thrustInput  = actions.ContinuousActions[1];

        rb.angularVelocity = -rotateInput * rotationSpeed;
        Vector2 thrustDirection = transform.up;
        rb.AddForce(thrustDirection * thrustInput * thrustForce);

        if (thrustInput < 0) {
            AddReward(-0.05f);
        }

        CheckRotationPenalty();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void MoveFoodTarget() {
        targetTransform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
    }

    void Update()
    {
        if (rb.velocity.magnitude > minimumVelocity)
        {
            Vector2 direction = rb.velocity.normalized;
            facingLeft = direction.x < 0;
            Flip();
        }
        
    } 

    private void CheckRotationPenalty()
    {
        // Get normalized velocities
        float angularSpeed = Mathf.Abs(rb.angularVelocity);
        float linearSpeed = rb.velocity.magnitude;

        // Check conditions for penalty
        float minimumVelocity = 20f;

        // Debug.Log("angularSpeed: " + angularSpeed + " linearSpeed: " + linearSpeed);
        if (angularSpeed > 20f && linearSpeed < minimumVelocity)
        {
            rotationPenaltyTimer += Time.deltaTime;
            
            // Apply increasing penalty over time
            if (rotationPenaltyTimer > 1f) // 1 second threshold
            {
                // Debug.Log("penalizare");
                AddReward(-0.5f * Time.deltaTime);
            }
        }
        else
        {
            rotationPenaltyTimer = 0f;
        }
    }

    private void Flip() {
        visuals.transform.localScale = facingLeft ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
    }

    public void WaterColor(bool normal_Color, Color color) {
        waterColor.color = normal_Color ? waterColor_color : color;
    }
}
