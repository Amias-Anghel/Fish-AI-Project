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
    private Color waterColor_color;

    private float health, hunger, stress, age;

    private Vector2 swimLocation;
    private float swimLocationTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        // transform.localPosition = Vector3.zero;
        // transform.localPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-28f, 18f), 0);
        // envObservator.SpawnFood();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(health);
        sensor.AddObservation(hunger);
        sensor.AddObservation(stress);
        sensor.AddObservation(age);
        
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        if (!envObservator.AddFoodObservations(sensor, transform.position)) {
            sensor.AddObservation(swimLocation.x);
            sensor.AddObservation(swimLocation.y);
        }

        envObservator.AddFishObservations(sensor, transform.position);
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

    void Update()
    {
        swimLocationTimer += Time.deltaTime;
        if (swimLocationTimer >= 1f || Vector2.Distance(swimLocation, transform.position) < 0.2f) {
            swimLocationTimer = 0;
            swimLocation = new Vector2(Random.Range(-50f, 50f), Random.Range(-28f, 18f));
        }

        age += Time.deltaTime;
        if (age > lifeExpentancy) {
            Destroy(gameObject);
        }
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
