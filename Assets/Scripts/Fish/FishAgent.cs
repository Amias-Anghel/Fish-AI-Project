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

    private float minimumVelocity = 0.4f;
    // [SerializeField] GameObject visuals;
    private bool facingLeft = true;
    
    
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
        // transform.localPosition = new Vector3(Random.Range(-40f, 40f), Random.Range(-20f, 15f), 0);
        transform.localPosition = Vector3.zero;
        MoveFoodTarget();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.z);
        
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(0);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotateInput  = actions.ContinuousActions[0];
        float thrustInput  = actions.ContinuousActions[1];

        // if (rotateInput < 0.2 && rotateInput > -0.2) rotateInput = 0f;
        // if (thrustInput < 0.2 && thrustInput > -0.6) thrustInput = 0f;

        rb.angularVelocity = -rotateInput * rotationSpeed;
        Vector2 thrustDirection = transform.up;
        rb.AddForce(thrustDirection * thrustInput * thrustForce);

        if (thrustInput < 0) {
            AddReward(-0.4f);
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
    }

    // void Update()
    // {
    //     if (rb.velocity.magnitude > minimumVelocity)
    //     {
    //         Vector2 direction = rb.velocity.normalized;
    //         facingLeft = direction.x < 0;
    //         Flip();
    //     }
        
    // }

    // private void Flip() {
    //     visuals.transform.localScale = facingLeft ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
    // }

    public void WaterColor(bool normal_Color, Color color) {
        waterColor.color = normal_Color ? waterColor_color : color;
    }
}
