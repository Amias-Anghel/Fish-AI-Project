using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SwimAgent : Agent
{
    [SerializeField] private AgentsManager agentsManager;
    private FishVisuals fishVisuals;
    public bool isTraining = true;

    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float minSpeed = 30f;
    private Rigidbody2D rb;

    [SerializeField] public EnvObservator envObservator;
    [SerializeField] public Transform head;

    // stats
    [Range(0f, 1f)] [SerializeField] private float hunger;
    [Range(0f, 1f)] [SerializeField] private float hungerTreshold = 0.5f;
    private bool hasTarget = true;

    // swiming around
    private Vector2 swimLocation;
    private float swimLocationTimer;

    void Awake()
    {
        fishVisuals = GetComponent<FishVisuals>();
        rb = GetComponent<Rigidbody2D>();

        hunger = 0;

        movementSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Start()
    {
        swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
        
    }

    public override void OnEpisodeBegin()
    {
        if (isTraining)
        {
            envObservator.MoveAllFoodTargets();
            hunger = Random.Range(0f, 1f);
            hungerTreshold = Random.Range(0f, 1f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // hunger - 2
        sensor.AddObservation(hunger);
        sensor.AddObservation(hungerTreshold);

        // fish position - 2
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);
        sensor.AddObservation(headRelativePos.x);
        sensor.AddObservation(headRelativePos.y);

        // SWIM OBSERVATIONS
        // swim position - 2
        sensor.AddObservation(swimLocation.x);
        sensor.AddObservation(swimLocation.y);
        // velocity - 2
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);

        // aq limits - 4
        envObservator.AddAquariumLimits(sensor);

        // attack decision taken into account
        bool attack = agentsManager.attackAgent != null ? agentsManager.attackAgent.GetAttackDecision() : false;
        if (attack)
        {
            hasTarget = envObservator.HasFishTarget();
        }
        else
        {
            hasTarget = envObservator.HasFoodTarget();
        }

        // food exists - 1
        sensor.AddObservation(hasTarget);

        // target direction - 2 && food/fish pos - 2
        Vector2 directionToTarget;

        if (hasTarget && attack)
        {
            Vector2 targetPos = envObservator.GetClosestFishPos(headRelativePos, transform);
            sensor.AddObservation(targetPos);
            directionToTarget = (targetPos - headRelativePos).normalized;
        }
        else if (hasTarget && hunger >= hungerTreshold)
        {
            Vector2 targetPos = envObservator.GetClosestFoodPos(headRelativePos);
            sensor.AddObservation(targetPos);
            directionToTarget = (targetPos - headRelativePos).normalized;
        }
        else
        {
            sensor.AddObservation(Vector2.zero);
            directionToTarget = (swimLocation - headRelativePos).normalized;
        }

        sensor.AddObservation(directionToTarget);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 movement;
        movement.x = actions.ContinuousActions[0];
        movement.y = actions.ContinuousActions[1];

        rb.velocity = movement * movementSpeed;

        if (rb.velocity.magnitude > 0.2f)
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
        if (!isTraining)
        {
            // hunger
            hunger += Time.deltaTime * 0.05f;
            hunger = hunger > 1 ? 1 : hunger;
        }

        // swiming
        CheckSwimPosition();

        if (isTraining)
        {
            AddReward(-0.001f);
        }
    }

    public void Eat()
    {
        if (isTraining)
        {
            if (hunger >= hungerTreshold) AddReward(1);
            EndEpisode();
        }

        fishVisuals.SetPoopTimer();
        hunger -= 0.5f;
        hunger = hunger < 0 ? 0 : hunger;
    }

    private void CheckSwimPosition()
    {
        swimLocationTimer += Time.deltaTime;
        Vector2 headRelativePos = transform.parent.InverseTransformPoint(head.position);

        float swimDestDist = 3f;
        float distToDest = Vector2.Distance(swimLocation, headRelativePos);

        if (isTraining)
        {
            if (hasTarget)
            {
                if (distToDest < swimDestDist)
                {
                    if (hunger < hungerTreshold) AddReward(1);
                    if (hunger > hungerTreshold) AddReward(-1);
                    EndEpisode();
                }
            }
            else
            {
                if (distToDest < swimDestDist)
                {
                    AddReward(1);
                    EndEpisode();
                }
            }
        }

        // select new swim location
        if (swimLocationTimer >= 20f || distToDest < swimDestDist)
        {
            swimLocationTimer = 0;
            swimLocation = transform.parent.InverseTransformPoint(envObservator.userLimits.GetPositionInAquarium());
        }
    }

    public Vector2 TrainingGetSwimPos()
    {
        return swimLocation;
    }

    public float GetHunger()
    {
        return hunger;
    }
    
    public float GetHungerThreshold() {
        return hungerTreshold;
    }
}