using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FishAgent : Agent
{
    [SerializeField] float moveSpeed = 20f;
    private float minimumVelocity = 0.4f;
    [SerializeField] GameObject visuals;
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
        // transform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        transform.localPosition = Vector3.zero;
        MoveFoodTarget();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // transform.localPosition += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;

        Vector2 movement = new Vector2(moveX, moveY);
        rb.velocity = movement * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Food>(out Food food)) {
            AddReward(1f);
            waterColor.color = waterColor_color;
            // EndEpisode();
            MoveFoodTarget();
        }

        if (collision.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-1f);
            waterColor.color = Color.grey;
            EndEpisode();
        }
    }

    public void OCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall)) {
            AddReward(-3f);
            waterColor.color = Color.grey;
            EndEpisode();
        }
    }

    private void MoveFoodTarget() {
        targetTransform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
    }

    void Update()
    {
        if (rb.velocity.magnitude > minimumVelocity)
        {
            Vector2 direction = rb.velocity.normalized;

            // Flip the sprite based on horizontal direction
            facingLeft = direction.x < 0;
            Flip();

            // Calculate the rotation angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            
            // Apply rotation
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Reset rotation when not moving
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        
    } 

    private void Flip() {
        visuals.transform.localScale = facingLeft ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
    }
}
